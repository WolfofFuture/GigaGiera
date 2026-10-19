using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 0.3f;     // zasieg trafienia liczony od osi obrotu miecza
    public float attackCooldown = 1f;

    public AudioClip swingSound; // dzwiek ataku, przypisz w Inspectorze
    private AudioSource audioSource;

    [Header("Ciecie (miecz wedruje po luku wokol gracza)")]
    public Vector2 pivotOffset = Vector2.zero;  // os obrotu wzgledem gracza (lokalnie)
    public float arcRadius = 0.17f;             // odleglosc srodka miecza od osi obrotu
    public float startAngle = 110f;             // poczatek ciecia: 0 = prosto do przodu, 90 = w gore
    public float endAngle = -100f;              // koniec ciecia
    public float windUpTime = 0.1f;
    public float cutTime = 0.26f;
    public float recoverTime = 0.18f;
    public float hitMarginAngle = 10f;          // tolerancja trafienia wokol ostrza, w stopniach

    [Header("Smuga (jeden ciagly polksiezyc w pikselach)")]
    public Color trailColor = new Color(0.75f, 1f, 1f, 1f);
    public float trailLength = 85f;             // dlugosc ogona w stopniach luku
    public float trailFadeTime = 0.14f;         // jak dlugo ogon zwija sie do ostrza po ciosie
    public float trailPixel = 0.0125f;          // rozmiar piksela smugi w jednostkach swiata
    public float trailInnerOffset = -0.01f;     // wewnetrzna krawedz wzgledem arcRadius
    public float trailOuterOffset = 0.075f;     // zewnetrzna krawedz wzgledem arcRadius (koniec ostrza)

    private float lastAttackTime = -Mathf.Infinity;

    private CooldownBar cooldownBar;
    private Animator anim;
    private SpriteRenderer swordRenderer;

    private Vector3 restPosition;
    private Quaternion restRotation;
    private bool swinging;
    private float swingFacing = 1f;
    private Mover swingMover;

    // smuga
    private SpriteRenderer trailRenderer;
    private Texture2D trailTexture;
    private Sprite trailSprite;
    private Color32[] trailBuffer;
    private int trailSize;
    private Coroutine trailFade;

    void Start()
    {
        // Ruch miecza liczymy sami, wiec Animator nie moze go nadpisywac.
        anim = GetComponent<Animator>();
        if (anim != null)
            anim.enabled = false;

        swordRenderer = GetComponent<SpriteRenderer>();
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;

        CreateTrail();

        // Pobranie AudioSource i ustawienie glosnosci z PlayerPrefs
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioSource.enabled = audioSource.volume > 0f;

        SFXVolumeSetter sfxSetter = GetComponent<SFXVolumeSetter>();
        if (sfxSetter == null)
        {
            sfxSetter = gameObject.AddComponent<SFXVolumeSetter>();
        }

        // Cooldown bar
        GameObject playerUi = GameObject.Find("playerUi");
        if (playerUi != null)
        {
            Transform cooldownBarTransform = playerUi.transform.Find("CooldownBar");
            if (cooldownBarTransform != null)
            {
                cooldownBar = cooldownBarTransform.GetComponent<CooldownBar>();
                if (cooldownBar != null)
                {
                    cooldownBar.SetCooldownProgress(1f);
                }
                else
                {
                    Debug.LogError("Brak skryptu CooldownBar na CooldownBar!");
                }
            }
            else
            {
                Debug.LogError("Nie znaleziono CooldownBar w playerUi!");
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono playerUi w scenie!");
        }
    }

    void Update()
    {
        float timeSinceLastAttack = Time.time - lastAttackTime;
        float progress = Mathf.Clamp01(timeSinceLastAttack / attackCooldown);

        if (cooldownBar != null)
        {
            cooldownBar.SetCooldownProgress(progress);
        }

        if (GameInput.Pressed(GameAction.Attack))
        {
            if (timeSinceLastAttack >= attackCooldown && !swinging)
            {
                lastAttackTime = Time.time;
                StartCoroutine(SwingRoutine());
                PlaySwingSound();
            }
            else
            {
                Debug.Log("Cooldown active, cannot attack yet!");
            }
        }
    }

    void OnDestroy()
    {
        if (swingMover != null)
            swingMover.lockFacing = false;
        if (trailRenderer != null)
            Destroy(trailRenderer.gameObject);
        if (trailSprite != null)
            Destroy(trailSprite);
        if (trailTexture != null)
            Destroy(trailTexture);
    }

    // Zamach, ciecie z gory na dol po luku i powrot do pozycji spoczynkowej.
    private IEnumerator SwingRoutine()
    {
        swinging = true;
        HashSet<Enemy> alreadyHit = new HashSet<Enemy>();

        // Kierunek ciosu jest ustalany na poczatku zamachu i trzymany do konca,
        // zeby szybka zmiana kierunku ruchu nie przerzucala luku na druga strone.
        Transform swingOwner = transform.parent != null ? transform.parent : transform;
        swingFacing = swingOwner.lossyScale.x < 0f ? -1f : 1f;
        swingMover = swingOwner.GetComponent<Mover>();
        if (swingMover != null)
            swingMover.lockFacing = true;

        if (trailFade != null)
        {
            StopCoroutine(trailFade);
            trailFade = null;
        }
        HideTrail();

        // Zamach: z pozycji spoczynkowej na poczatek luku.
        Vector3 startPos;
        Quaternion startRot;
        ArcPose(startAngle, out startPos, out startRot);
        yield return Blend(restPosition, restRotation, startPos, startRot, windUpTime);

        // Ciecie: miecz wedruje po luku, ciagnie za soba smuge i trafia wrogow.
        float elapsed = 0f;
        float previous = startAngle;

        while (elapsed < cutTime)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / cutTime);
            float eased = k * k * (3f - 2f * k);
            float angle = Mathf.Lerp(startAngle, endAngle, eased);

            Vector3 pos;
            Quaternion rot;
            ArcPose(angle, out pos, out rot);
            transform.localPosition = pos;
            transform.localRotation = rot;

            HitAlongArc(previous, angle, alreadyHit);
            PaintTrail(angle, 1f);

            previous = angle;
            yield return null;
        }

        Vector3 endPos;
        Quaternion endRot;
        ArcPose(endAngle, out endPos, out endRot);
        transform.localPosition = endPos;
        transform.localRotation = endRot;
        HitAlongArc(previous, endAngle, alreadyHit);
        PaintTrail(endAngle, 1f);

        // Ogon zwija sie do ostrza rownolegle z powrotem miecza.
        trailFade = StartCoroutine(FadeTrail(endAngle));

        yield return Blend(endPos, endRot, restPosition, restRotation, recoverTime);

        transform.localPosition = restPosition;
        transform.localRotation = restRotation;
        if (swingMover != null)
            swingMover.lockFacing = false;
        swinging = false;
    }

    // Pozycja i obrot miecza na luku: srodek miecza lezy na okregu, ostrze skierowane na zewnatrz.
    private void ArcPose(float angleDeg, out Vector3 position, out Quaternion rotation)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 p = pivotOffset + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * arcRadius;
        position = new Vector3(p.x, p.y, restPosition.z);
        rotation = Quaternion.Euler(0f, 0f, angleDeg - 90f);
    }

    private IEnumerator Blend(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / time);
            float eased = k * k * (3f - 2f * k);
            transform.localPosition = Vector3.Lerp(fromPos, toPos, eased);
            transform.localRotation = Quaternion.Slerp(fromRot, toRot, eased);
            yield return null;
        }
    }

    // Trafia wrogow lezacych w pasie, ktory ostrze przeciely miedzy poprzednia a obecna klatka.
    private void HitAlongArc(float previousAngle, float currentAngle, HashSet<Enemy> alreadyHit)
    {
        Transform owner = transform.parent != null ? transform.parent : transform;
        Vector3 pivot = owner.TransformPoint(pivotOffset);
        float facing = swingFacing;

        float minAngle = Mathf.Min(previousAngle, currentAngle) - hitMarginAngle;
        float maxAngle = Mathf.Max(previousAngle, currentAngle) + hitMarginAngle;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pivot, attackRange);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || alreadyHit.Contains(enemy))
                continue;

            Vector2 offset = hit.ClosestPoint(pivot) - (Vector2)pivot;
            bool inside = offset.sqrMagnitude < 0.000001f;
            float angle = Mathf.Atan2(offset.y, offset.x * facing) * Mathf.Rad2Deg;

            if (inside || (angle >= minAngle && angle <= maxAngle))
            {
                alreadyHit.Add(enemy);
                enemy.TakeDamage(damage);
                Debug.Log("Gracz trafil potwora za " + damage + " dmg.");
            }
        }
    }

    // ---- smuga ----

    // Maly obrazek rysowany piksel po pikselu (filtr punktowy), pokazywany jako sprite na osi obrotu miecza.
    private void CreateTrail()
    {
        Transform owner = transform.parent != null ? transform.parent : transform;

        float outer = arcRadius + trailOuterOffset;
        trailSize = Mathf.Max(8, Mathf.CeilToInt(2f * (outer + trailPixel) / trailPixel));
        trailBuffer = new Color32[trailSize * trailSize];

        trailTexture = new Texture2D(trailSize, trailSize, TextureFormat.RGBA32, false);
        trailTexture.filterMode = FilterMode.Point;
        trailTexture.wrapMode = TextureWrapMode.Clamp;

        trailSprite = Sprite.Create(trailTexture, new Rect(0, 0, trailSize, trailSize), new Vector2(0.5f, 0.5f), 1f / trailPixel);

        GameObject go = new GameObject("SwordTrail");
        go.transform.SetParent(owner, false);
        go.transform.localPosition = new Vector3(pivotOffset.x, pivotOffset.y, restPosition.z);

        trailRenderer = go.AddComponent<SpriteRenderer>();
        trailRenderer.sprite = trailSprite;
        if (swordRenderer != null)
        {
            trailRenderer.sharedMaterial = swordRenderer.sharedMaterial;
            trailRenderer.sortingLayerID = swordRenderer.sortingLayerID;
            trailRenderer.sortingOrder = swordRenderer.sortingOrder - 1;
        }
        trailRenderer.enabled = false;
    }

    private void HideTrail()
    {
        if (trailRenderer != null)
            trailRenderer.enabled = false;
    }

    // Maluje polksiezyc: pasmo na okregu wokol osi, od aktualnego kata ostrza (glowa) do tylu (ogon).
    // Ogon jest ograniczony do juz przebytego luku, a jasnosc spada skokowo w trzech poziomach.
    private void PaintTrail(float headAngle, float shrink)
    {
        if (trailRenderer == null)
            return;

        float tail = Mathf.Min(trailLength, startAngle - headAngle) * shrink;
        float inner = arcRadius + trailInnerOffset;
        float outer = arcRadius + trailOuterOffset;
        float half = trailSize * 0.5f;
        bool any = false;

        for (int j = 0; j < trailSize; j++)
        {
            for (int i = 0; i < trailSize; i++)
            {
                float dx = (i + 0.5f - half) * trailPixel;
                float dy = (j + 0.5f - half) * trailPixel;
                float r = Mathf.Sqrt(dx * dx + dy * dy);

                byte alpha = 0;
                if (tail > 0.5f && r >= inner && r <= outer)
                {
                    float phi = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                    float d = phi - headAngle;
                    if (d >= 0f && d <= tail)
                    {
                        float t = d / tail;
                        alpha = (byte)(t < 0.34f ? 165 : (t < 0.67f ? 100 : 45));
                        any = true;
                    }
                }

                trailBuffer[j * trailSize + i] = new Color32(
                    (byte)(trailColor.r * 255f), (byte)(trailColor.g * 255f), (byte)(trailColor.b * 255f), alpha);
            }
        }

        trailTexture.SetPixels32(trailBuffer);
        trailTexture.Apply(false);
        trailRenderer.enabled = any;
    }

    private IEnumerator FadeTrail(float headAngle)
    {
        float elapsed = 0f;
        while (elapsed < trailFadeTime)
        {
            elapsed += Time.deltaTime;
            float shrink = 1f - Mathf.Clamp01(elapsed / trailFadeTime);
            PaintTrail(headAngle, shrink);
            yield return null;
        }

        HideTrail();
        trailFade = null;
    }

    private void PlaySwingSound()
    {
        // Dzwiek ataku zaczynajacy sie od 0.2 sekundy
        if (swingSound != null && audioSource != null && audioSource.enabled)
        {
            audioSource.clip = swingSound;
            audioSource.time = 0.2f;
            audioSource.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Transform owner = transform.parent != null ? transform.parent : transform;
        Gizmos.DrawWireSphere(owner.TransformPoint(pivotOffset), attackRange);
    }
}
