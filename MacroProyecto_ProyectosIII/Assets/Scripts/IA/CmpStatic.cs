using AIEngine.Movement.Agents;
using AIEngine.Movement.Algorithms;
using AIEngine.Movement.Components.Algorithms;
using UnityEngine;


namespace AIEngine.Movement.Components.Agents
{
    public class CmpStatic : MonoBehaviour
    {
        AgStatic agent;
        MonoBehaviour algorithmComponent;

        float maxSpeed;

        private void Awake()
        {
            agent = new AgStatic(
                new System.Numerics.Vector2(transform.position.x, transform.position.z),
                transform.rotation.y
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
                // Si no tiene Seek, sincroniza su posición con el transform (es un objetivo o estático)
                agent.position = new System.Numerics.Vector2(transform.position.x, transform.position.z);
            }

            // Siempre actualiza la posición del GameObject con respecto al agent
            transform.position = new Vector3(agent.position.X, 0, agent.position.Y);

            float angle = -agent.orientation * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        public AgStatic GetAgent() { return agent; }
    }
}
