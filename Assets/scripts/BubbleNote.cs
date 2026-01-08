using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BubbleNote : MonoBehaviour
{
    private float spawnTime;
    private float lifeTime;
    private float targetTime; 

    private LineRenderer lineRenderer;
    private bool isHit = false;

    private int segments = 50;
    private float startRadiusScale = 3.0f; 

    private Camera mainCamera;
    private static Texture2D softVideoLineTex; // Cached Texture

    public void Initialize(float lifeTime)
    {
        this.lifeTime = lifeTime;
        this.spawnTime = Time.time;
        this.targetTime = spawnTime + lifeTime;
        
        mainCamera = Camera.main; 
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        
        // [FIX] NEON GLOW GENERATION (Procedural Soft Texture)
        if (softVideoLineTex == null) {
            softVideoLineTex = new Texture2D(1, 32);
            for (int y = 0; y < 32; y++) {
                // Bell Curve style fade (0 -> 1 -> 0)
                float v = Mathf.Sin((y / 31f) * Mathf.PI); 
                softVideoLineTex.SetPixel(0, y, new Color(1, 1, 1, v));
            }
            softVideoLineTex.Apply();
            softVideoLineTex.wrapMode = TextureWrapMode.Clamp;
        }

        // 1. Thicker line to accommodate the fade
        lineRenderer.startWidth = 0.4f; 
        lineRenderer.endWidth = 0.4f;
        
        lineRenderer.positionCount = segments + 1;
        
        // 2. Additive Shader + Soft Texture
        var shader = Shader.Find("Particles/Additive");
        if (shader == null) shader = Shader.Find("Mobile/Particles/Additive");
        
        var mat = new Material(shader != null ? shader : Shader.Find("Sprites/Default"));
        mat.mainTexture = softVideoLineTex; // Assign Soft Texture
        lineRenderer.material = mat;
        // [FIX] TRANSPARENT GLOWING CYAN VISUALS
        var meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            // Original Bright White-Cyan Color (as it was before)
            Color cyanNeon = new Color(0.8f, 0.9f, 1f); // Brighter, more white
            
            // Create Transparent Glowing Material
            var beadMat = new Material(Shader.Find("Standard"));
            
            // Enable Transparent Mode
            beadMat.SetFloat("_Mode", 3); // Transparent
            beadMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            beadMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            beadMat.SetInt("_ZWrite", 0);
            beadMat.DisableKeyword("_ALPHATEST_ON");
            beadMat.EnableKeyword("_ALPHABLEND_ON");
            beadMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            beadMat.renderQueue = 3000;
            
            // Set transparent base color (70% opacity - 조금 투명)
            beadMat.color = new Color(cyanNeon.r, cyanNeon.g, cyanNeon.b, 0.7f);
            
            // Bright Emission for neon glow
            beadMat.EnableKeyword("_EMISSION");
            beadMat.SetColor("_EmissionColor", cyanNeon * 2.5f); // Bright cyan glow
            beadMat.SetFloat("_Glossiness", 0.9f); // Shiny
            
            meshRenderer.material = beadMat;
        }

        // [FIX] Initial Scale 0 for Pop Animation
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (isHit) return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckInput();
        }

        float timeAlive = Time.time - spawnTime;
        float progress = timeAlive / lifeTime;

        // [FIX] BUBBLE POP-IN ANIMATION
        // Grow from 0 to 1 over first 0.4 seconds (or 20% of life)
        float popDuration = 0.4f; 
        if (timeAlive < popDuration)
        {
            float popT = timeAlive / popDuration;
            // Elastic Ease Out for "Bubble" feel
            // t = sin(13 * pi * 0.5 * t) * pow(2, 10 * (t - 1)) + 1 (Too complex?)
            // Simple EaseOutBack: c1*t*t*t + c2*t + ...
            // Let's stick to simple smoothstep or Lerp
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2.0f, Mathf.SmoothStep(0, 1, popT));
        }
        else
        {
             transform.localScale = Vector3.one * 2.0f;
        }

        float timeRemaining = targetTime - Time.time;
        if (timeRemaining <= 0.5f)
        {
            // [FIX] Sky Blue Neon Judgement Ring
            // Changed from Green to Cyan/Sky Blue as requested
            lineRenderer.startColor = new Color(0.2f, 0.9f, 1f, 0.9f); 
            lineRenderer.endColor = new Color(0.2f, 0.9f, 1f, 0.9f);
            
            // Pulse Effect
            float pulse = Mathf.PingPong(Time.time * 8, 0.15f);
            lineRenderer.startWidth = 0.5f + pulse; 
            lineRenderer.endWidth = 0.5f + pulse;
        }
        else
        {
            // Approaching Ring - NEON STYLE
            // White with transparency, but Additive shader makes it glow.
            // Width is thick (0.35) to look like a light tube.
            lineRenderer.startColor = new Color(0.8f, 0.9f, 1f, 0.5f); // Soft Cyan-ish White
            lineRenderer.endColor = new Color(0.8f, 0.9f, 1f, 0.5f);
            lineRenderer.startWidth = 0.35f; lineRenderer.endWidth = 0.35f;
        }

        if (progress >= 1.0f)
        {
            OnMiss();
        }
        else
        {
            float currentScale = Mathf.Lerp(startRadiusScale, 1.0f, progress);
            DrawCircle(currentScale);
        }
    }

    private void CheckInput()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform) 
            {
                OnMouseDownManual(); 
            }
        }
    }

    private void DrawCircle(float scaleRadius)
    {
        float xradius = 0.5f * scaleRadius;
        float yradius = 0.5f * scaleRadius;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }

    private void OnMouseDownManual() 
    {
        if (isHit) return;

        float timeRemaining = targetTime - Time.time;
        
        if (Mathf.Abs(timeRemaining) < 0.5f) 
        {
            OnHit();
        }
    }

    private void OnHit()
    {
        isHit = true;
        
        // [FIX] Shrink-Explode Effect
        StartCoroutine(ShrinkExplodeEffect());
        
        Color effectColor = Color.white;

        if (GameManager.Instance != null)
        {
            float timeDiff = Mathf.Abs(targetTime - Time.time);
            
            if (timeDiff <= 0.15f)
            {
                GameManager.Instance.AddPerfect(transform.position);
                effectColor = new Color(0, 1, 1); // Cyan
            }
            else if (timeDiff <= 0.3f)
            {
                GameManager.Instance.AddGreat(transform.position);
                effectColor = Color.green;
            }
            else
            {
                GameManager.Instance.AddBad(transform.position);
                effectColor = new Color(1, 0.5f, 0); // Orange
            }
        }
        
        GameObject splashObj = new GameObject("SplashEffect");
        splashObj.transform.position = transform.position;
        // Basic Particle System
        ParticleSystem ps = splashObj.AddComponent<ParticleSystem>();
        var renderer = splashObj.GetComponent<ParticleSystemRenderer>();
        
        // [FIX] Assign Material to prevent Pink "Missing Material" Glitch
        renderer.material = new Material(Shader.Find("Sprites/Default")); 
        
        // [FIX] Particle Refinement: "Small Glowing Dots"
        var main = ps.main;
        main.startColor = effectColor;
        // Small glowing dots
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4f); 
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f); // Tiny dots
        main.startLifetime = 0.4f; // Short life (0.4s)
        main.loop = false;
        
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0; // Burst only
        emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0f, 15) }); // 15 particles

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f; // Small source radius
        
        // Disable Velocity over Lifetime defaults if any, to ensure "Burst" feel
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local; // Fix mismatch error
        vel.radial = 1f; // Explode outward
        
        // Cleanup
        Destroy(splashObj, 1.5f);
        Destroy(gameObject);
    }

    private void OnMiss()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMiss(transform.position); 
        }
        Destroy(gameObject);
    }

    // [FIX] Shrink-Explode Visual Effect
    private System.Collections.IEnumerator ShrinkExplodeEffect()
    {
        Vector3 originalScale = transform.localScale;
        float shrinkDuration = 0.1f;
        float explodeDuration = 0.15f;
        
        // Phase 1: Shrink to 70%
        float elapsed = 0;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.7f, t);
            yield return null;
        }
        
        // Phase 2: Explode to 150% then fade
        elapsed = 0;
        while (elapsed < explodeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / explodeDuration;
            transform.localScale = Vector3.Lerp(originalScale * 0.7f, originalScale * 1.5f, t);
            
            // Fade out renderer
            var rend = GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                Color c = rend.material.color;
                c.a = Mathf.Lerp(0.7f, 0f, t); // Start from 70% opacity
                rend.material.color = c;
            }
            yield return null;
        }
    }
}
