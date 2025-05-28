using AIEngine.Movement.Output;
using System.Numerics;

namespace AIEngine.Movement.Agents
{
    public class AgStatic
    {
        public Vector2 position;
        public float orientation;

        public AgStatic(Vector2 position, float orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }

        public void Update(SteeringOutput sOut,float dt)
        {
            position += sOut.linear * dt;
            orientation += sOut.angular * dt;
        }
    }
}
