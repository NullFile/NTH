using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Altair
{
    public class SoundControl : MemoryPoolingFlag, ISoundPlay //인터페이스 상속
    {
        private new AudioSource audio; //AudioSource Component를 불러온다.
        [SerializeField] private AudioClip clip; //AudioClip을 가져온다. 
        private bool isFirstPlay = true; //true로 초기화 한다.

        private void Awake() //반드시 Awake에서 처리한다.
        {
            audio = GetComponent<AudioSource>(); //AudioSource를 GetComponent
            if (clip == null) clip = Resources.Load<AudioClip>("리소스 폴더 내 경로"); //혹시 에디터에서 미리 연결해놓지 않은 경우를 위한 예외 처리
        }

        private void OnEnable()
        {
            if (isFirstPlay) isFirstPlay = false; //첫 번째 실행인 경우 소리를 재생하지 않고 boolen만 true로 전환한다.
            else SoundPlay(ref clip);  //첫 번째 실행이 아닌 경우 정상적으로 소리를 재생한다.
        }

        public void SoundPlay(ref AudioClip clip) //인터페이스 명시적 구현
        {
            if (audio == null) return; //AudioSource를 가져오지 않았을 경우를 대비한 예외 처리

            audio.Stop(); //혹시 아직 재생 중인 효과음이 있을 경우 정지
            audio.PlayOneShot(clip); //소리를 재생한다.
        }
    }
}