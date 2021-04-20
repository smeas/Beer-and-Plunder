using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Player;

namespace Menu {

    public class PlayerSlotObject : MonoBehaviour
    {

        public int Id;

        private bool isTaken;
        public bool IsTaken => isTaken;

        private PlayerComponent playerComponent;
        public PlayerComponent PlayerComponent => playerComponent;

        [SerializeField] TextMeshProUGUI joinText;
        [SerializeField] TextMeshProUGUI readyText;

        private Image background;

	    private void Start() {
            background = GetComponent<Image>();
	    }

	    public void JoinPlayer(PlayerComponent playerComponent) {

            if (isTaken)
                return;

            readyText.gameObject.SetActive(true);
            joinText.gameObject.SetActive(false);

            background.color = Color.green;
            isTaken = true;

            this.playerComponent = playerComponent;
	    }

        public void LeavePlayer() {
            readyText.gameObject.SetActive(false);
            joinText.gameObject.SetActive(true);

            background.color = Color.gray;
            isTaken = false;
        }
    }
}

