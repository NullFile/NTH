using UnityEngine;
using UnityEngine.UI;
using Altair;

namespace Choi
{
    public class ConfigMgr : MonoBehaviour
    {
        public Button closeBtn;
        public Button OkBtn;

        public Button volumeBtn;
        public Slider volumeSlider;
        public Sprite[] volumeBtnSprites;

        public InputField nickNameField;
        public Text infoText;
        float infoTimer = -0.1f;


        private void Start() => StartFunc();

        private void StartFunc()
        {
            if(volumeSlider != null)
            {
                volumeSlider.value = GlobalData.masterVolume;

                volumeSlider.onValueChanged.AddListener((float val) =>
                {
                    GlobalData.masterVolume = val;
                    if (val > 0)
                    {
                        volumeBtn.image.sprite = volumeBtnSprites[1];
                        PlayerPrefs.SetInt("volumeisOn", 1);
                    }
                    else
                    {
                        volumeBtn.image.sprite = volumeBtnSprites[0];
                        PlayerPrefs.SetInt("volumeisOn", 0);

                    }

                    PlayerPrefs.SetFloat("masterVolume", GlobalData.masterVolume);
                });
            }

            if (volumeBtn != null)
            {
                if (GlobalData.volumeisOn)
                    volumeBtn.image.sprite = volumeBtnSprites[1];
                else
                    volumeBtn.image.sprite = volumeBtnSprites[0];

                volumeBtn.onClick.AddListener(() =>
                {
                    GlobalData.volumeisOn = !GlobalData.volumeisOn;

                    if (GlobalData.volumeisOn)
                    {
                        volumeBtn.image.sprite = volumeBtnSprites[1];
                        PlayerPrefs.SetInt("volumeisOn", 1);
                        GlobalData.volumeisOn = true;
                    }
                    else
                    {
                        volumeBtn.image.sprite = volumeBtnSprites[0];
                        PlayerPrefs.SetInt("volumeisOn", 0);
                        GlobalData.volumeisOn = false;
                    }

                });
            }


            if(nickNameField != null)
            {
                nickNameField.placeholder.GetComponent<Text>().text = "플레이어 닉네임";
            }

            if (closeBtn != null)
                closeBtn.onClick.AddListener(() =>
                {
                    this.gameObject.SetActive(false);
                });

            if (OkBtn != null)
                OkBtn.onClick.AddListener(() =>
                {
                    if(nickNameField.text != "플레이어 닉네임" && nickNameField.text != "")
                    {
                        Debug.Log(nickNameField.text + "로 변경완료");
                        infoText.gameObject.SetActive(true);
                        nickNameField.placeholder.GetComponent<Text>().text = nickNameField.text;
                        nickNameField.text = "";
                        infoTimer = 7.0f;
                    }
                });
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if(infoTimer >= 0)
            {
                infoTimer -= Time.deltaTime;
                if(infoTimer <= 0.0f)
                {
                    infoText.gameObject.SetActive(false);
                }
            }
        }
    }
}