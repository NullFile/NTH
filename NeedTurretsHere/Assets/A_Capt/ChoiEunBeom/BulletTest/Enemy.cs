using UnityEngine;
using Altair;
using Altair_Memory_Pool_Pro;

namespace Choi
{
    public class Enemy : MemoryPoolingFlag, IDamageable
    {
        public int maxhp = 5;
        public int hp = 5;
        public float speed = 1;

        public float slowTimer = 0.0f;

        SkinnedMeshRenderer _skRenderer;
        Material _skMat;

        MeshRenderer[] _msRenderer;
        Material[] _msMat;

		private void OnEnable()
		{
            hp = maxhp;
		}

		private void Start() => StartFunc();

        private void StartFunc()
        {
            _skRenderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            _skMat = _skRenderer.material;

            _msRenderer = this.gameObject.GetComponentsInChildren<MeshRenderer>();
            _msMat = new Material[_msRenderer.Length];

            for (int i = 0; i < _msRenderer.Length; i++)
            {
                _msMat[i] = _msRenderer[i].material;
            }

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (this.transform.position.x >= -5)
                this.transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (slowTimer >= 0.0f)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0.0f)
                {
                    speed = 1.0f;
                    slowFunc(false);
                }
            }
        }

        public void slowFunc(bool isSlow)
        {
            if (isSlow)
            {
                _skMat.color = new Color32(0, 0, 255, 0);
                for (int i = 0; i < _msMat.Length; i++)
                {
                    _msMat[i].color = new Color32(0, 0, 255, 0);
                }
            }

            else
            {
                _skMat.color = new Color32(0, 0, 0, 0);
                for (int i = 0; i < _msMat.Length; i++)
                {
                    _msMat[i].color = new Color32(0, 0, 0, 0);
                }
            }
        }

        public void OnDamage(int dam)
        {
            hp -= dam;

            if (hp <= 0)
            {
                ObjectReturn();
            }
        }
    }
}