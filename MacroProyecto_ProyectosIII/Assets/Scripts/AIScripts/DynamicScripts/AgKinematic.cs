using AIEngine.Movement.Output;
using System.Numerics;

namespace AIEngine.Movement.Agents
{
    public class AgKinematic
    {
        public Vector2 position;
        public float orientation;

        public Vector2 velocity;
        public float rotation;

        public AgKinematic(Vector2 position, float orientation, Vector2 velocity, float rotation)
        {
            this.position = position;
            this.orientation = orientation;
            this.velocity = velocity;
            this.rotation = rotation;
        }

        public void Update(SteeringOutput sOut, float dt)
        {
            // Actualiza la velocidad con la aceleración proporcionada
            velocity += sOut.linear * dt;
            rotation += sOut.angular * dt;

            // Aplica el movimiento
            position += velocity * dt;
            orientation += rotation * dt;
        }
    }
}
