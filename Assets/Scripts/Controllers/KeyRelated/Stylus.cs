
namespace Controller
{
    namespace Key
    {
#pragma warning disable 649
        public class Stylus : AbstractSimple
        {
            [UnityEngine.SerializeField]
            private TMPro.TMP_Text textMeshPro;

            public override string text
            {
                get => textMeshPro.text;
                set => textMeshPro.text = value;
            }
        }
    }
}