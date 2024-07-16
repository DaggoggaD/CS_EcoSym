using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class OldCilia : Component
    {
        public float ActivationValue;
        public float CostPerTime;
        public float Speed;
        public float RotationSpeed;
        public bool Initialized = false;

        public float input;

        public OldCilia()
        {
            componentFamilyName = "ROTATE";
        }

        public void Initialize()
        {
            ActivationValue = components_stats[0];
            CostPerTime = components_stats[1];
            Speed = components_stats[2];
            RotationSpeed = components_stats[3];
        }

        public override dynamic RUN(List<GameElement> cells)
        {
            if (Initialized == false) Initialize();
            if (input == float.NegativeInfinity) return null;
            if (inputV < 0)
            {
                cellParent.angularVelocity -= 0.001f*input;
            }
            else
            {
                cellParent.angularVelocity+= 0.001f*input;
            }

            return null;
        }
    }
}
