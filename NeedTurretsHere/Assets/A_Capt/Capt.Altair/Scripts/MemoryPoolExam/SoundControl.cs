using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Altair
{
    public class SoundControl : MemoryPoolingFlag, ISoundPlay //�������̽� ���
    {
        private new AudioSource audio; //AudioSource Component�� �ҷ��´�.
        [SerializeField] private AudioClip clip; //AudioClip�� �����´�. 
        private bool isFirstPlay = true; //true�� �ʱ�ȭ �Ѵ�.

        private void Awake() //�ݵ�� Awake���� ó���Ѵ�.
        {
            audio = GetComponent<AudioSource>(); //AudioSource�� GetComponent
            if (clip == null) clip = Resources.Load<AudioClip>("���ҽ� ���� �� ���"); //Ȥ�� �����Ϳ��� �̸� �����س��� ���� ��츦 ���� ���� ó��
        }

        private void OnEnable()
        {
            if (isFirstPlay) isFirstPlay = false; //ù ��° ������ ��� �Ҹ��� ������� �ʰ� boolen�� true�� ��ȯ�Ѵ�.
            else SoundPlay(ref clip);  //ù ��° ������ �ƴ� ��� ���������� �Ҹ��� ����Ѵ�.
        }

        public void SoundPlay(ref AudioClip clip) //�������̽� ����� ����
        {
            if (audio == null) return; //AudioSource�� �������� �ʾ��� ��츦 ����� ���� ó��

            audio.Stop(); //Ȥ�� ���� ��� ���� ȿ������ ���� ��� ����
            audio.PlayOneShot(clip); //�Ҹ��� ����Ѵ�.
        }
    }
}