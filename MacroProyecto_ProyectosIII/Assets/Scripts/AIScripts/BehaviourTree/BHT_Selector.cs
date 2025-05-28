using UnityEngine;

namespace AIEngine.Decision.BehaviourTree.Tasks
{
    public class BHT_Selector : BHT_Task
    {
        private readonly BHT_Task[] childrens;

        public BHT_Selector(BHT_Task[] childrens)
        {
            this.childrens = childrens;
        }

        public override bool Run()
        {
            foreach (var child in childrens) 
            {
                if (child.Run())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
