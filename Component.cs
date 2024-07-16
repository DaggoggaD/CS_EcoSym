using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace C_EcoSimApp
{
    internal class Component
    {
        public string STRcomponent_ID;
        public List<string> STRcomponents_stats;
        public string stroldcmpid;

        public string componentFamilyName;
        public string componentType;
        public int component_ID;
        public List<float> components_stats;
        public string name;

        public Cell cellParent;
        public List<float> results = new List<float>();
        public float inputV = 0;
        public float inputN = 0;
        public float CostXtime = 0;
        public string DebugName = "Comp";

        public static Dictionary<string, string> componentsName = new Dictionary<string, string>
        {
            {"00000", "SightPlants"},
            {"00001", "SightMeat"},
            {"00010", "SightAmmino"},
            {"00011", "SmellPlantConc"},
            {"00100", "SmellAmminoConc"},
            {"00101", "SmellPhero"},
            {"00110", "PosX"},
            {"00111", "PosY"},
            {"01000", "EatPlantSPC"},
            {"01001", "EatMeatSPC"},
            {"01010", "EatAmminoSPC"},
            {"01011", "EatNotSPC"},
            {"01100", "EatEnclousure"},
            {"01101", "Flagellum"},
            {"01110", "Cilia"},
            {"01111", "PheromoneRelease"},
            {"10000", "Spike"},
        };

        public Component FindComponent()
        {
            Component foundComponent;
            switch (component_ID)
            {
                case 0:
                    foundComponent = new Sight(nameof(Cell));
                    break;
                case 1:
                    foundComponent = new Sight(nameof(Plant));
                    break;
                case 2:
                    foundComponent = new Sight(nameof(GameElement));
                    break;
                case 3:
                    foundComponent = new Cilia();
                    break;
                case 4:
                    foundComponent = new Flagellum();
                    break;
                case 5:
                    foundComponent = new PheroRelease();
                    break;
                case 6:
                    foundComponent = new Pheromone();
                    break;
                case 7:
                    foundComponent = new Position();
                    break;
                case 8:
                    foundComponent = new Position();
                    break;
                case 9:
                    foundComponent = new Pheromone();
                    break;
                case 10:
                    foundComponent = new Cilia();
                    break;
                case 11:
                    foundComponent = new Flagellum();
                    break;
                case 12:
                    foundComponent = new Pheromone();
                    break;
                case 13:
                    foundComponent = new Position();
                    break;
                case 14:
                    foundComponent = new Cilia();
                    break;
                case 15:
                    foundComponent = new Flagellum();
                    break;
                case 16:
                    foundComponent = new Pheromone();
                    break;
                default:
                    throw new Exception("Component_NOT_found");
            }
            
            return foundComponent;
        }

        public Component TranslateComponent()
        {
            //CONVERT string componentID and check if in dictionary range
            if (STRcomponent_ID[0].ToString()=="1" && (STRcomponent_ID.LastIndexOf("1") != -1))
            {
                string str = "0";
                for (int i = 1;i<STRcomponent_ID.Length;i++)
                {
                    str+= STRcomponent_ID[i];
                }
                STRcomponent_ID = str;

                
            }
            component_ID = Convert.ToInt32(STRcomponent_ID, 2);
            //Convert string component stats
            List<float> stats = new List<float>();
            foreach (string stat in STRcomponents_stats)
            {
                float res = 10*  Convert.ToInt32(stat, 2) / ((float)Math.Pow(2, stat.Length) - 1);
                stats.Add(res);
            }

            //Finds component type, sets stats, name and id
            Component foundComponent = FindComponent();
            foundComponent.component_ID = component_ID;
            foundComponent.components_stats = stats;
            foundComponent.name = componentsName[STRcomponent_ID];
            return foundComponent;
        }

        public string REPR()
        {
            string res = "";
            if (components_stats != null)
            {
                components_stats.ForEach(s => res += s.ToString() + ", ");
            }
            return $"COMPONENT: {name}({component_ID}, {STRcomponent_ID}); stats: [{res}]\n";
        }

        public virtual dynamic RUN(List<GameElement> elements)
        {
            return null;
        }
    }
}
