using AIEngine.Movement.Agents;
using AIEngine.Movement.Output;
using System;
using System.ComponentModel;
using System.Numerics;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

namespace AIEngine.Movement.Algorithms
{
    public class DynamicFaceAlgorithm:DynamicAlignAlgorithm
    {
        private AgKinematic agent;
        private AgKinematic target;

        private float maxAngularAcceleration;
        private float maxRotation;
        private float targetRange;
        private float decelerationRange;
        private float timeToTarget;
        public DynamicFaceAlgorithm(float maxAngularAcceleration, float maxRotation, float targetRange, float decelerationRange, float timeToTarget)
            : base(maxAngularAcceleration, maxRotation, targetRange, decelerationRange, timeToTarget)
        {
        }

        public new void SetAgent(AgKinematic agent) => this.agent = agent;
        public new void SetTarget(AgKinematic target) => this.target = target;

        public override SteeringOutput GetSteering()
        {
            Vector2 direction = target.position - agent.position;

            // Si la dirección es prácticamente cero, no girar
            if (direction.LengthSquared() < 0.001f)
            {
                return new SteeringOutput(); // Sin rotación
            }

            // Convertir dirección a orientación deseada (en radianes)
            float desiredOrientation = (float)Math.Atan2(direction.X, direction.Y); // X, Y porque usamos plano XZ

            // Crear un target ficticio con esa orientación
            AgKinematic faceTarget = new AgKinematic(agent.position, desiredOrientation, Vector2.Zero, 0f);

            // Aplicar alineación hacia esa orientación
            base.SetAgent(agent);
            base.SetTarget(faceTarget);

            return base.GetSteering();
        }
    }
}
