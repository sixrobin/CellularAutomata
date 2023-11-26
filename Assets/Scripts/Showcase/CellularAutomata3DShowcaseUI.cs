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
            // TODO: Insert space in texts to make them easier to read.
            this._nameText.text = settings.name;
            this._rulesText.text = settings.Rules;
        }

        private void Start()
        {
            this._nameText.text = "";
            this._rulesText.text = "";
        }
    }
}