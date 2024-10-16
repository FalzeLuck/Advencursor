using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class RandomLoop<T>
    {
        private List<T> items;
        private List<T> shuffled;
        private Random random;
        private int currentIndex;

        public RandomLoop(List<T> items)
        {
            this.items = new List<T>(items);
            ShuffleItems(); 
            this.currentIndex = 0;
        }

        private void ShuffleItems()
        {
            shuffled = new List<T>(items);
            int n = shuffled.Count;
            for (int i = 0; i < n; i++)
            {
                int j = Globals.random.Next(i, n);
                T temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }
        }

        public T GetNext()
        {
            if (currentIndex >= shuffled.Count)
            {
                ShuffleItems();
                currentIndex = 0;
            }

            return shuffled[currentIndex++];
        }
    }
}
