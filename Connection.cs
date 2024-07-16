using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class Connection
    {
        public string InputOrHiddenLayer;
        public string HiddenLayerOrOutput;
        public int connectionStart;
        public int connectionEnd;
        public float weight;
        public float bias;
        public float optionStart;
        public float optionEnd;


        public Connection(string cs, string ce, string w, string b, string os, string oe) {
            if (cs != "")
            {
                if (cs[0].ToString() == "0")
                {
                    InputOrHiddenLayer = "Input";
                }
                else InputOrHiddenLayer = "HiddenLayer";

                if (ce[0].ToString() == "0")
                {
                    HiddenLayerOrOutput = "HiddenLayer";

                }
                else HiddenLayerOrOutput = "Output";
                connectionStart = Convert.ToInt32(cs.Substring(1), 2);
                connectionEnd = Convert.ToInt32(ce.Substring(1), 2);
                weight = (float)Convert.ToInt32(w, 2) / 64 - 1;
                bias = (float)Convert.ToInt32(b, 2) / 64 - 1;
                optionStart = (float)Convert.ToInt32(os, 2);
                optionEnd = (float)Convert.ToInt32(oe, 2);
            }
        }

        public string REPR()
        {
            return $"Connection from: {InputOrHiddenLayer}({connectionStart}) to {HiddenLayerOrOutput}({connectionEnd}).\n\tWeight: {weight}, \n\tBias: {bias};\n";
        }

    }
}
