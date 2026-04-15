using UnityEngine;

public enum EnemyType
{
    Patrol,
    Idle,
    Demon
}

public class EnemyBrain : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public FPSController playerScript;

    [Header("Patrulla")]
    public PatrolPath patrolPath;


    [Header("Demon Audios")]
    public Transform posVentana;
    public Transform posPuerta;
    public Transform posCama;
    public AudioClip clipVentana, clipPuerta, clipCama;



    public EnemyType type;

    [HideInInspector] public VisionSensor vision;
    [HideInInspector] public HearingSensor hearing;
    [HideInInspector] public MemorySystem memory;
    [HideInInspector] public AlertSystem alert;
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public EnemyUI enemyUI;

    void Awake()
    {
        vision = GetComponent<VisionSensor>();
        hearing = GetComponent<HearingSensor>();
        memory = GetComponent<MemorySystem>();
        alert = GetComponent<AlertSystem>();
        stateMachine = GetComponent<StateMachine>();
        enemyUI = GetComponent<EnemyUI>();
    }

    void Start()
    {
        switch (type)
        {
            case EnemyType.Patrol:
                stateMachine.ChangeState(new PatrolState(this));
                break;

            case EnemyType.Idle:
                stateMachine.ChangeState(new IdleState(this));
                break;
            
            case EnemyType.Demon:
                stateMachine.ChangeState(new DemonState(this));
                break;
        }

    }

    void Update()
    {
        var result = vision.CheckVision(player);
        var detection = GetComponent<DetectionSystem>();

        if (result == VisionSensor.VisionResult.Clear)
        {
            detection.IncreaseDetection(true);
            memory.SetLastKnownPosition(player.position);
        }
        else if (result == VisionSensor.VisionResult.Suspicious)
        {
            detection.IncreaseDetection(false);
            memory.SetLastKnownPosition(player.position);
        }
        else
        {
            detection.DecreaseDetection();
        }

        // DETECCIÓN COMPLETA
        if (detection.IsFullyDetected)
        {
            alert.SetAlertLevel(AlertLevel.Combat);
        }
        // SOSPECHA
        else if (detection.IsSuspicious)
        {
            alert.SetAlertLevel(AlertLevel.Suspicious);
        }

        //  sonido
        if (hearing.CanHearSomething())
        {
            memory.SetLastKnownPosition(hearing.LastHeardPosition);
            alert.SetAlertLevel(AlertLevel.Suspicious);
        }
    }
}