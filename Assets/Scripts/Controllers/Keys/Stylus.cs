namespace Controller.Key
{
#pragma warning disable 649
    public class Stylus : AbstractSimple
    {
        [UnityEngine.SerializeField]
        private TMPro.TMP_Text textMeshPro;

        public override string text
        {
            get { return textMeshPro.text; }
            set { textMeshPro.text = value; }
        }
    }
}