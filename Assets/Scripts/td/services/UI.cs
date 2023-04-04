using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace td.services
{
    public class UI
    {
        private readonly Label livesLebel;
        private readonly Label moneyLebel;
        private readonly Label waveLabel;
        private readonly Label waveCountdownLabel;

        private readonly Regex oneNumberRegex = new Regex(@"[\d#.-]+");
        private readonly Regex waveRegex = new Regex(@"(\d+|#+)/(\d+|#+)");


        public UI()
        {
            var root = Object.FindObjectOfType<UIDocument>().rootVisualElement;
            
            livesLebel = root.Query<Label>("lives").First();
            moneyLebel = root.Query<Label>("money").First();
            waveLabel = root.Query<Label>("wave").First();
            waveCountdownLabel = root.Query<Label>("waveCountdown").First();
            
            Debug.Assert(livesLebel != null);
            Debug.Assert(moneyLebel != null);
            Debug.Assert(waveLabel != null);
            Debug.Assert(waveCountdownLabel != null);
        }

        public void UpdateLives(int lives)
        {
            livesLebel.text = oneNumberRegex.Replace(livesLebel.text, lives.ToString());
        }

        public void UpdateMoney(int money)
        {
            moneyLebel.text = oneNumberRegex.Replace(moneyLebel.text, money.ToString());
        }

        public void UpdateWave(int waveNumber, int maxWaves)
        {
            if (waveNumber != 0 || maxWaves != 0)
            {
                waveLabel.visible = true;
                waveLabel.text = waveRegex.Replace(waveLabel.text, $@"{waveNumber}/{maxWaves}");
            }
            else
            {
                waveLabel.visible = false;
            }
        }

        public void UpdateWaveCountdown(int countdown)
        {
            if (countdown > 0)
            {
                waveCountdownLabel.visible = true;
                waveCountdownLabel.text = oneNumberRegex.Replace(waveCountdownLabel.text, countdown.ToString());
            }
            else
            {
                waveCountdownLabel.visible = false;
            }
        }
    }
}