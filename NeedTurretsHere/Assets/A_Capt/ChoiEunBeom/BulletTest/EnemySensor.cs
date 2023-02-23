using UnityEngine;

namespace Choi
{
    public class EnemySensor : MonoBehaviour
    {

        //public GameObject firstObj;
        //RaycastHit2D hit;
        //public LayerMask enemyLayer;

        //[SerializeField] GameObject bulletPrefab;
        //Collider2D collider;

        //float minDistance = 999.0f;

        private void Start() => StartFunc();

        private void StartFunc()
        {
         
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            //hit = Physics2D.Raycast(this.transform.position, Vector2.right, 20.0f, enemyLayer);

            //if(hit)
            //{
            //    firstObj = hit.collider.gameObject;
            //}

            //if(Input.GetKeyDown(KeyCode.Space))
            //{
            //    GameObject bullet = Instantiate(bulletPrefab);
            //    bullet.transform.position = this.transform.position;
            //    if(bullet.TryGetComponent(out BulletCtrl ctrl))
            //    {
            //        ctrl.enemySensor = this;
            //    }
            //}
        }




        //public void GetDistance(float distance, GameObject enemyobj)
        //{
        //    //Debug.Log(distance + " : " + enemyobj.name + " : " + minDistance);

        //    if(distance <= minDistance)
        //    {
        //        firstObj = enemyobj;
        //        minDistance = distance;
        //    }
        //}
    }
}