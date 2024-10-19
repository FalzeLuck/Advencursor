using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Advencursor._Managers
{
    

    public class DialogueManager
    {
        private DialogueData dialogueData;

        public DialogueManager(string filepath)
        {
            string jsonContent = File.ReadAllText(filepath);
            dialogueData = JsonSerializer.Deserialize<DialogueData>(jsonContent);
        }

        public List<Dialogue> GetSceneDialogue(string sceneID)
        {
            foreach (var scene in dialogueData.scenes)
            {
                if (scene.sceneID == sceneID)
                {
                    return scene.dialogues;
                }
            }
            return null;
        }
    }


    public class Dialogue
    {
        public string speaker { get; set; }
        public string text { get; set; }
    }

    public class Scene
    {
        public string sceneID { get; set; }
        public List<Dialogue> dialogues { get; set; }
    }

    public class DialogueData
    {
        public List<Scene> scenes { get; set; }
    }
}
