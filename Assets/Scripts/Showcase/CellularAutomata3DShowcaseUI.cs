namespace CellularAutomata.Showcase
{
    using TMPro;
    using UnityEngine;

    public class CellularAutomata3DShowcaseUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _rulesText;

        public void SetSettings(CellularAutomatonSettings settings)
        {
            this._nameText.text = settings.DisplayName;
            this._rulesText.text = settings.Rules.Replace("/", " / ").Trim();
        }

        private void Start()
        {
            this._nameText.text = "";
            this._rulesText.text = "";
        }
    }
}