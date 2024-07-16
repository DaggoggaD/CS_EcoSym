using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class GenomeHandler
    {
        internal static string RandomBits(int lenght, Random random)
        {
            string res = "";
            for (int i = 0; i < lenght; i++)
            {
                int currbit = random.Next(0, 2);
                currbit.ToString();
                res += currbit.ToString();
            }
            return res;
        }

        internal static Genome GenerateGenome(int componentID_len, int floatACC_len, int components_len, int connections_len)
        {
            string wholegenome = "";
            Random random = new Random();
            //Components
            List<Component> components = new List<Component>();
            for (int i = 0;i<components_len; i++)
            {
                //component generation, could be improved by not initializing two components (see "*")
                string ComponentID = RandomBits(componentID_len, random);
                wholegenome += ComponentID;
                List<string> ComponentStats = new List<string>();
                for (int j = 0; j < 5; j++)
                {
                    string currentstat = RandomBits(floatACC_len, random);
                    wholegenome += currentstat;
                    ComponentStats.Add(currentstat);
                }
                //*
                Component strcomponent = new Component();
                strcomponent.STRcomponent_ID = ComponentID;
                strcomponent.STRcomponents_stats = ComponentStats;

                //*
                Component component = strcomponent.TranslateComponent();
                components.Add(component);
            }

            
            //Connections
            List<Connection> connections = new List<Connection>();
            for (int i = 0;i<connections_len; i++)
            {
                string connectionStart = RandomBits(5, random);
                string connectionEND = RandomBits(5, random);
                string weight = RandomBits(floatACC_len,random);
                string bias = RandomBits(floatACC_len, random);
                //Small Option Start/End
                string optionStart = RandomBits(1, random);
                string optionEnd = RandomBits(1, random);
                wholegenome += connectionStart + weight + bias + connectionEND + optionStart + optionEnd;

                Connection connection = new Connection(connectionStart,connectionEND, weight, bias, optionStart, optionEnd);
                connections.Add(connection);
            }

            Genome genome = new Genome(components, connections, wholegenome);
            return genome;
        }
    }
}
