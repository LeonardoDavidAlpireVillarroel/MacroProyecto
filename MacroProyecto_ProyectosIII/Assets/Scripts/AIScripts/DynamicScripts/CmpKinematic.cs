using AIEngine.Movement.Agents;
using UnityEngine;


namespace AIEngine.Movement.Components.Agents
{
    public class CmpKinematic : MonoBehaviour
    {
        AgKinematic agent;
        MonoBehaviour algorithmComponent;

        private Vector3 previousPosition;

        private void Awake()
        {

            agent = new AgKinematic(
                new System.Numerics.Vector2(transform.position.x, transform.position.z),
                transform.rotation.eulerAngles.y * Mathf.Deg2Rad,
                System.Numerics.Vector2.Zero,
                0f
            );

            algorithmComponent = GetComponent<ISteeringProvider>() as MonoBehaviour;

        }

        private void Update()
        {
            if (algorithmComponent != null && algorithmComponent is ISteeringProvider steeringProvider)
            {
                agent.Update(steeringProvider.GetSteering(), Time.deltaTime);
            }
            else
            {
                Vector3 currentPosition = transform.position;

                Vector3 displacement = currentPosition - previousPosition;

                Vector3 velocity = (currentPosition - previousPosition) / Time.deltaTime;

                agent.velocity = new System.Numerics.Vector2(velocity.x, velocity.z);

                agent.position = new System.Numerics.Vector2(currentPosition.x, currentPosition.z);

                agent.orientation = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

                previousPosition = currentPosition;


            }

            transform.position = new Vector3(agent.position.X, 0, agent.position.Y);

            float angle = agent.orientation * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        public AgKinematic GetAgent() { return agent; }
    }
}
