using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Text EarnCash_txt;
    public Animator WinMenu_anim;
    public GameObject Win_Menu;
    public GameObject Pause_Button;
    public int money = 0;
    public Text moneyText;
    [SerializeField] private List<string> sceneNames;
    private string currentSceneName;
    private static int lastLoadedSceneIndex = -1;

    public GameObject gameoverEnd;
    public AudioSource finalDeliverySound;
    public ParticleSystem boxDeliverParticle;
    public AudioSource boxDeliverSound;
    public Transform finalPlayerTarget;
    public float boxSpread = 0.5f;
    public Animator Run;
    public AudioSource myAudio;

    [Header("Particles")]
    public ParticleSystem moneyParticles;
    public ParticleSystem confettiParticles;
    public Transform deliverTarget;
    private bool followPlayer = true;

    [Header("Movement Settings")]
    public bool go;
    public float speed = 5f;
    public float roadEndPoint = 3f;
    public float playerZspeed = 15f;
    public float camSpeed = 0.4f;

    [Header("References")]
    public GameObject startObject;
    public GameObject Player;
    public GameObject GameOver;
    public List<GameObject> allowedStartObjects;

    [Header("Body Parts")]
    public GameObject bodyPrefab;
    public int gap = 2;
    public float bodySpeed = 15f;

    [Header("Animations")]
    public Animator Obs1;

    private Transform player;
    private Camera mainCam;
    private float velocity;
    private float camVelocity;
    private Vector3 offset;
    private Vector3 firstMousePos;
    private Vector3 firstPlayerPos;
    private bool moveTheBall;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<int> bodyPartsIndex = new List<int>();
    private List<Vector3> PositionHistory = new List<Vector3>();

    private bool gameFinished = false;
    private int boxesDelivered = 0;

    void Start()
    {
        InitializePlayer();
        currentSceneName = SceneManager.GetActiveScene().name;

        int currentMoney = PlayerPrefs.GetInt("Money", 0);
        UpdateMoneyUI(currentMoney);

        if (moneyParticles != null)
            moneyParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (confettiParticles != null)
            confettiParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void InitializePlayer()
    {
        mainCam = Camera.main;
        player = transform;

        offset = mainCam != null ? mainCam.transform.position - player.position : new Vector3(0, 0, -10);
        offset.y = 0;

        moveTheBall = false;
        go = false;
        velocity = 0f;
        camVelocity = 0f;
        firstMousePos = Vector3.zero;
        firstPlayerPos = player.position;

        PositionHistory.Clear();
        PositionHistory.Add(player.position);

        foreach (var body in bodyParts)
        {
            if (body != null) Destroy(body);
        }
        bodyParts.Clear();
        bodyPartsIndex.Clear();

        player.position = new Vector3(0, player.position.y, player.position.z);
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        if (gameFinished) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI() && !IsPointerOverAllowedObject()) return;
            StartGame();
        }

        if (Input.GetMouseButtonUp(0)) moveTheBall = false;
    }

    private void HandleMovement()
    {
        if (moveTheBall && go)
        {
            if (mainCam == null) mainCam = Camera.main;
            if (mainCam == null) return;

            Plane newPlane = new Plane(Vector3.up, 0.8f);
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (newPlane.Raycast(ray, out var distance) && distance < 100f)
            {
                Vector3 newMousePos = ray.GetPoint(distance);
                if (float.IsNaN(newMousePos.x)) newMousePos.x = 0;
                if (float.IsNaN(newMousePos.z)) newMousePos.z = 0;

                Vector3 moveDelta = newMousePos - firstMousePos;
                Vector3 newPlayerPos = firstPlayerPos + moveDelta;
                newPlayerPos.x = Mathf.Clamp(newPlayerPos.x, -roadEndPoint, roadEndPoint);

                if (!float.IsNaN(newPlayerPos.x))
                {
                    player.position = new Vector3(
                        Mathf.SmoothDamp(player.position.x, newPlayerPos.x, ref velocity, speed * Time.deltaTime),
                        player.position.y,
                        player.position.z
                    );
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (go)
        {
            player.position += Vector3.forward * playerZspeed * Time.fixedDeltaTime;
            PositionHistory.Insert(0, player.position);

            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (bodyParts[i] == null) continue;
                Vector3 point = PositionHistory[Mathf.Min(i * gap, PositionHistory.Count - 1)];
                Vector3 moveDir = point - bodyParts[i].transform.position;
                bodyParts[i].transform.position += moveDir * bodySpeed * Time.fixedDeltaTime;
                bodyParts[i].transform.LookAt(point);
            }
        }
    }

    private void LateUpdate()
    {
        if (mainCam == null || !followPlayer) return;

        Vector3 targetCamPos = player.position + offset;
        targetCamPos.y = mainCam.transform.position.y;

        mainCam.transform.position = new Vector3(
            Mathf.SmoothDamp(mainCam.transform.position.x, targetCamPos.x, ref camVelocity, camSpeed * Time.deltaTime),
            targetCamPos.y,
            player.position.z + offset.z
        );
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GrowBody()
    {
        if (bodyPrefab == null) return;
        GameObject body = Instantiate(bodyPrefab, transform.position, transform.rotation);
        bodyParts.Add(body);
        bodyPartsIndex.Add(0);
    }

    private void StartGame()
    {
        Run.SetTrigger("run1");
        Pause_Button.SetActive(true);
        if (gameFinished) return;
        moveTheBall = true;
        go = true;
        if (startObject != null) startObject.SetActive(false);

        if (mainCam == null) mainCam = Camera.main;
        if (mainCam != null)
        {
            Plane newPlane = new Plane(Vector3.up, 0.8f);
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (newPlane.Raycast(ray, out var distance))
            {
                firstMousePos = ray.GetPoint(distance);
                firstPlayerPos = player.position;
            }
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return false;
    }

    private bool IsPointerOverAllowedObject()
    {
        if (EventSystem.current == null || allowedStartObjects == null) return false;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            if (allowedStartObjects.Contains(result.gameObject)) return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BoxObs"))
        {
            Destroy(other.gameObject, 0.005f);
            myAudio.Play();
            GrowBody();
        }
        else if (other.CompareTag("OBS1") && Obs1 != null)
        {
            Obs1.SetTrigger("Play");
        }
        
        else if (other.CompareTag("Finish"))
        {
            StartCoroutine(FinishSequence());
        }
    }

    IEnumerator MoveAndDisappear(GameObject body, Vector3 target)
    {
        float t = 0f;
        Vector3 startPos = body.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            body.transform.position = Vector3.Lerp(startPos, target, t);
            body.transform.LookAt(target);
            yield return null;
        }

        if (boxDeliverSound != null)
            boxDeliverSound.Play();

        if (boxDeliverParticle != null)
        {
            ParticleSystem ps = Instantiate(boxDeliverParticle, body.transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        boxesDelivered++;
        Destroy(body);

        int currentMoney = PlayerPrefs.GetInt("Money", 0);
        currentMoney += 1;
        PlayerPrefs.SetInt("Money", currentMoney);
        PlayerPrefs.Save();

        UpdateMoneyUI(currentMoney);
    }

    IEnumerator FinishSequence()
    {
        gameFinished = true;
        go = false;
        moveTheBall = false;
        followPlayer = false;

        if (bodyParts.Count < 5)
        {
            if (gameoverEnd != null)
                gameoverEnd.SetActive(true);
            Time.timeScale = 0;
            TryVibrate();
            yield break;
        }

        Run.SetTrigger("idle1");

        Vector3 startCamPos = mainCam.transform.position;
        Vector3 endCamPos = startCamPos + new Vector3(0, 5f, -5f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(startCamPos, endCamPos, t);
            yield return null;
        }

        Run.SetTrigger("run2");
        Vector3 targetPlayerPos = finalPlayerTarget.position;
        t = 0f;
        Vector3 startPlayerPos = player.position;
        while (t < 1f)
        {
            t += Time.deltaTime;
            player.position = Vector3.Lerp(startPlayerPos, targetPlayerPos, t);
            yield return null;
        }
        Run.SetTrigger("dance");

        int totalEarned = 0;
        int groupSize = 3;
        for (int i = 0; i < bodyParts.Count; i += groupSize)
        {
            List<GameObject> group = new List<GameObject>();
            for (int j = 0; j < groupSize && i + j < bodyParts.Count; j++)
            {
                group.Add(bodyParts[i + j]);
            }

            foreach (GameObject body in group)
            {
                if (body == null) continue;
                Vector3 randomOffset = new Vector3(Random.Range(-boxSpread, boxSpread), 0, Random.Range(-boxSpread, boxSpread));
                Vector3 target = deliverTarget.position + randomOffset;
                StartCoroutine(MoveAndDisappear(body, target));
                totalEarned += 1; 
                if (EarnCash_txt != null)
                    EarnCash_txt.text = totalEarned.ToString()+ "$" ; 
            }
            yield return new WaitForSeconds(0.2f);
        }

        while (boxesDelivered < bodyParts.Count)
        {
            yield return null;
        }

        if (moneyParticles != null) moneyParticles.Play();
        if (confettiParticles != null) confettiParticles.Play();
        if (finalDeliverySound != null) finalDeliverySound.Play();
        TryVibrate();

        yield return new WaitForSeconds(3f);
        Win_Menu.SetActive(true);
        WinMenu_anim.SetTrigger("play");
    }

    public void LoadRandomScene()
    {
        if (sceneNames == null || sceneNames.Count == 0)
        {
            Debug.LogError("No scene names assigned!");
            return;
        }

        int randomIndex;
        do {
            randomIndex = Random.Range(0, sceneNames.Count);
        } while (sceneNames.Count > 1 && randomIndex == lastLoadedSceneIndex);

        lastLoadedSceneIndex = randomIndex;
        SceneManager.LoadScene(sceneNames[randomIndex]);
    }

    public void SetVibrationFromToggle(bool isOn)
    {
        PlayerPrefs.SetInt("VibrationEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void TryVibrate()
    {
        if (PlayerPrefs.GetInt("VibrationEnabled", 1) == 1)
        {
            Handheld.Vibrate();
        }
    }

    public void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }

}