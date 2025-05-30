using AIEngine.Decision.BehaviourTree;
using AIEngine.Decision.BehaviourTree.Tasks;
using AIEngine.Movement.Components.Algorithms;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public GameObject entity;
    private BHT_Task tree;

    private EnemiPrueba enemiPrueba;
    private Animator animator;
    private Transform playerTransform;

    public float meleeDistance = 1.5f;
    public float launchDistance = 4.5f;
    public float perceptionDistance = 8f;

    private void Awake()
    {
        enemiPrueba = GetComponent<EnemiPrueba>();
        animator = enemiPrueba.animator;
        playerTransform = entity.transform;

        PathAssignerToPathFollowingPoints pathAssigner = GetComponent<PathAssignerToPathFollowingPoints>();
        CmpPathFollowing cmpPathFollowing = GetComponent<CmpPathFollowing>();

        if (pathAssigner == null || cmpPathFollowing == null || animator == null || enemiPrueba == null)
        {
            Debug.LogError("[Npc] Faltan componentes esenciales.");
            return;
        }

        tree = new BHT_Selector
        (
            new BHT_Task[]
            {
                new BHT_Sequence(new BHT_Task[]
                {
                    new IsEntityInDistance(entity, gameObject, meleeDistance),
                    new MeleeAttack(gameObject, animator, playerTransform, enemiPrueba)
                }),
                new BHT_Sequence(new BHT_Task[]
                {
                    new IsEntityInDistance(entity, gameObject, launchDistance),
                    new LaunchAtObjective(cmpPathFollowing, playerTransform, launchDistance, meleeDistance)
                }),
                new BHT_Sequence(new BHT_Task[]
                {
                    new EntityNear(perceptionDistance, entity, gameObject),
                    new ChaseObjective(pathAssigner, cmpPathFollowing, entity, perceptionDistance)
                })
            }
        );
    }

    private void Update()
    {
        if (enemiPrueba.isStuned) return;
        tree?.Run();
    }
}
