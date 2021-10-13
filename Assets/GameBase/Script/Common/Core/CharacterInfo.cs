
using UnityEngine;

namespace GameBase.Common.Core
{
    [System.Serializable]
    public class CharacterInfo
    {
        [SerializeField]
        public int id;
        [SerializeField]
        public string name;
        [SerializeField]
        public Gender gender;
        [SerializeField]
        public int birthYear;
        [SerializeField]
        public int age;
        [SerializeField]
        public string stateWords;
        [SerializeField]
        public SourceType imageType;
        [SerializeField]
        public string imageUrl;
        [SerializeField]
        public int imageId;
    }
}