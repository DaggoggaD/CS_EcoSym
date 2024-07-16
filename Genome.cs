using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class Genome
    {
        public List<Component> components;
        public List<Connection> connections;
        string wholeGenome;

        internal Genome(List<Component> COMPONENTS, List<Connection> CONNECTIONS, string WHOLEGENOME)
        {
            components = COMPONENTS;
            connections = CONNECTIONS;
            wholeGenome = WHOLEGENOME;
        }

        public string REPR()
        {
            string res = "";
            components.ForEach(c => res += c.REPR());
            connections.ForEach(c => res += c.REPR());
            return res;
        }
    }
}
