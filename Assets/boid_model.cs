using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boid_model : MonoBehaviour
{
    
    private float ray_long;
    private float visibility_radi; //個体の視野の半径
    private float keep_distance; //維持すべき距離
    private float max_speed;
    private float min_speed;
    private float target_weight;
    private float keep_dist_weight;
    private float follow_weight;
    private float ahead_center_weight;
    public float  avoid_obst_weight;
    private Ray ray; //個体正面の障害物検知
    private Vector3 velocity; //個体の速度
    private float speed; //個体の速さ
    private Vector3 accel; //個体の加速度
    private Vector3 flock_center; //群れの重心
    private Manager Manager_script; 
    private List<GameObject> children;


    // Start is called before the first frame update
    void Start()
    {
        //各パラメータをManagerから取得
        //Managerオブジェクトを取得
        GameObject Manager = GameObject.Find("Manager");
        //Managerオブジェクト内のscriptコンポーネントを取得
        Manager_script = Manager.GetComponent<Manager>();
        //Managerオブジェクト内のscriptコンポーネントから情報を取得
        float indiv_range = Random.Range(-3f, 3f); //個体差
        ray_long = Manager_script.ray_long;
        visibility_radi = Manager_script.visibility_radi + indiv_range;
        keep_distance = Manager_script.keep_distance + indiv_range;
        max_speed = Manager_script.max_speed;
        min_speed = Manager_script.min_speed;
        target_weight = Manager_script.target_weight;
        keep_dist_weight = Manager_script.keep_dist_weight;
        follow_weight = Manager_script.follow_weight;
        ahead_center_weight = Manager_script.ahead_center_weight;
        avoid_obst_weight = Manager_script.avoid_obst_weight;
        children = Manager_script.children;

        //視野を設定
        this.GetComponent<SphereCollider>().radius = visibility_radi;
        //前方障害物を避ける
        ray = new Ray(this.transform.position, this.GetComponent<Rigidbody>().velocity.normalized);
        //初速度を設定
        accel = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        this.GetComponent<Rigidbody>().velocity = culc_velocity(accel);
    }

    // Update is called once per frame
    void Update()
    {
    //inspectorの値を反映させるよう
        GameObject Manager = GameObject.Find("Manager");
        //Managerオブジェクト内のscriptコンポーネントを取得
        Manager_script = Manager.GetComponent<Manager>();
        //Managerオブジェクト内のscriptコンポーネントから情報を取得
        ray_long = Manager_script.ray_long;
        visibility_radi = Manager_script.visibility_radi;
        keep_distance = Manager_script.keep_distance;
        max_speed = Manager_script.max_speed;
        min_speed = Manager_script.min_speed;
        target_weight = Manager_script.target_weight;
        keep_dist_weight = Manager_script.keep_dist_weight;
        follow_weight = Manager_script.follow_weight;
        ahead_center_weight = Manager_script.ahead_center_weight;
        avoid_obst_weight = Manager_script.avoid_obst_weight;
        children = Manager_script.children;

        //視野を設定
        this.GetComponent<SphereCollider>().radius = visibility_radi;
        //前方障害物を避ける
        ray = new Ray(this.transform.position, this.GetComponent<Rigidbody>().velocity.normalized);

    //フレーム開始時の速度情報
        velocity = this.GetComponent<Rigidbody>().velocity;
        speed = velocity.magnitude;
        Vector3 normalized_velocity = velocity / speed;

    //このフレームでの新たな加速度を求める
        accel = Vector3.zero;
        
        //正面障害物検知
        ray = new Ray(this.transform.position, normalized_velocity * ray_long);
        Debug.DrawRay(this.transform.position, normalized_velocity * ray_long, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, ray_long)){
            if(hit.collider.gameObject.name != "fish"){
                accel += culc_avoid_obstacle();
            }
        }
        
        accel += culc_ahead_center();
        accel += culc_keep_dist_nearest_accel();

    //フレーム終了時の速度情報の確定
        velocity = 0.95f * this.GetComponent<Rigidbody>().velocity + 0.05f * culc_velocity(accel);
        speed = velocity.magnitude;
        this.transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up); //速度の方を向く
        //最終的な移動速度を指定範囲内に収める
        this.GetComponent<Rigidbody>().velocity =  velocity.normalized * Mathf.Clamp(speed, min_speed, max_speed);
    }

    //Updateとは別で、視野内に入った時に処理される
    /*
    void OnTriggerEnter(Collider other){
        if(other.name == "Cube"){
            //Start関数よりも早く実行されるため、初速度が定まってない状態で始まりエラーになってしまう。それを防ぐためのif
            if(this.GetComponent<Rigidbody>().velocity.magnitude != 0){
                //フレーム開始時の速度情報
                velocity = this.GetComponent<Rigidbody>().velocity;
                speed = velocity.magnitude;
                Vector3 normalized_velocity = velocity / speed;

            //このフレームでの新たな加速度を求める
                accel = Vector3.zero;
                accel += culc_ahead_near_direc_accel(other); 
                accel += culc_keep_dist_accel(other);

            //フレーム終了時の速度情報の確定
                velocity = 0.95f * this.GetComponent<Rigidbody>().velocity + 0.05f * culc_velocity(accel);
                speed = velocity.magnitude;
                this.transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up); //速度の方を向く
                //最終的な移動速度を指定範囲内に収める
                this.GetComponent<Rigidbody>().velocity =  velocity.normalized * Mathf.Clamp(speed, min_speed, max_speed);
                Debug.Log("入った");
            }
        }
    }
*/
    void OnTriggerStay(Collider other){
        if(other.name == "fish"){
            //Start関数よりも早く実行されるため、初速度が定まってない状態で始まりエラーになってしまう。それを防ぐためのif
            if(this.GetComponent<Rigidbody>().velocity.magnitude != 0){
                //フレーム開始時の速度情報
                velocity = this.GetComponent<Rigidbody>().velocity;
                speed = velocity.magnitude;
                Vector3 normalized_velocity = velocity / speed;

            //このフレームでの新たな加速度を求める
                accel = Vector3.zero;
                accel += culc_ahead_near_direc_accel(other); 
                accel += culc_keep_dist_accel(other);

            //フレーム終了時の速度情報の確定
                velocity = 0.95f * this.GetComponent<Rigidbody>().velocity + 0.05f * culc_velocity(accel);
                speed = velocity.magnitude;
                this.transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up); //速度の方を向く
                //最終的な移動速度を指定範囲内に収める
                this.GetComponent<Rigidbody>().velocity =  velocity.normalized * Mathf.Clamp(speed, min_speed, max_speed);
                //Debug.Log("入った");
            }
        }
    }
    /*
    void OnTriggerExit(Collider other){
        if(other.name == "Cube"){
            //Start関数よりも早く実行されるため、初速度が定まってない状態で始まりエラーになってしまう。それを防ぐためのif
            if(this.GetComponent<Rigidbody>().velocity.magnitude != 0){
                //フレーム開始時の速度情報
                velocity = this.GetComponent<Rigidbody>().velocity;
                speed = velocity.magnitude;
                Vector3 normalized_velocity = velocity / speed;

            //このフレームでの新たな加速度を求める
                accel = Vector3.zero;
                accel += culc_ahead_near_direc_accel(other); 
                accel += culc_keep_dist_accel(other);

            //フレーム終了時の速度情報の確定
                velocity = 0.95f * this.GetComponent<Rigidbody>().velocity + 0.05f * culc_velocity(accel);
                speed = velocity.magnitude;
                this.transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up); //速度の方を向く
                //最終的な移動速度を指定範囲内に収める
                this.GetComponent<Rigidbody>().velocity =  velocity.normalized * Mathf.Clamp(speed, min_speed, max_speed);
                Debug.Log("出た");
            }
        }
    }
*/
    private Vector3 culc_velocity(Vector3 accel){
        //与えられた加速度から瞬間速度を求める
        Vector3 velocity = accel * Time.deltaTime;
        
        return velocity;
    }

    private Vector3 culc_ahead_near_direc_accel(Collider boid){
        Vector3 accel = Vector3.zero;
        //最も近くにいるboidの進行方向へ進もうとする加速度を得る
        accel = boid.GetComponent<Rigidbody>().velocity.normalized * follow_weight;
        return accel;
    }  

    private Vector3 culc_keep_dist_accel(Collider boid){
        Vector3 accel = Vector3.zero;
        Vector3 diff = boid.transform.position - this.transform.position; //相手方向へ向くベクトルがもとまる
        if(diff.magnitude > keep_distance){
            //相手座標へのベクトルを加速度とし、keep_dist_weightで調整する。 diffが大きいほど加速度が大きくなる。
            float ratio = diff.magnitude / keep_distance;
            accel = keep_dist_weight * ratio * diff.normalized;
        }else if(diff.magnitude < keep_distance){
            //−１をかけることで、相手から自分方向へ、つまり離れる方向へ加速するようにする。 diffが小さいほど加速度が大きくなる
            float ratio = keep_distance / diff.magnitude;
            accel = keep_dist_weight * ratio * diff.normalized * -1;
        }
        return accel;
    }

    private Vector3 culc_keep_dist_nearest_accel(){
        Vector3 accel = Vector3.zero;
        GameObject nearest_boid = find_nearest_boid().transform.GetChild(0).gameObject;
        Vector3 diff = nearest_boid.transform.position - this.transform.position; //相手方向へ向くベクトルがもとまる
        if(diff.magnitude > keep_distance){
            //相手座標へのベクトルを加速度とし、keep_dist_weightで調整する。 diffが大きいほど加速度が大きくなる。
            float ratio = diff.magnitude / keep_distance;
            accel = keep_dist_weight * ratio * diff.normalized;
        }else if(diff.magnitude < keep_distance){
            //−１をかけることで、相手から自分方向へ、つまり離れる方向へ加速するようにする。 diffが小さいほど加速度が大きくなる
            float ratio = keep_distance / diff.magnitude;
            accel = keep_dist_weight * ratio * diff.normalized * -1;
        }
        return accel;
    }   

    private Vector3 culc_ahead_center(){ //中心に向かうベクトルを返す
        flock_center = Manager_script.flock_center;
        float diff_center = (flock_center - this.transform.position).magnitude;
        Vector3 accel = ahead_center_weight * (flock_center - this.transform.position).normalized * diff_center; //diff_centerが大きいほど、加速が大きくなる
        return accel; //重心の方を向いたベクトルを返す
    }

    private GameObject find_nearest_boid(){
        children = Manager_script.children;
        float nearest_dist = 10000000f;
        GameObject nearest_child = null;
        foreach(GameObject child in children){
            //自分自身の時は無視
            if(this.transform.parent.gameObject == child){
                continue;
            }
            
            //任意のchildとのの距離を計算する
            Transform child_t = child.transform.GetChild(0);
            GameObject child_ob = child_t.gameObject;
            Vector3 dist = this.transform.position - child_ob.transform.position;
            //現在の最小距離よりオブジェクト間の距離が小さい場合, 最も近い他のboidを更新
            if(dist.magnitude < nearest_dist){
                nearest_dist = dist.magnitude;
                nearest_child = child;
            }            
        }
        return nearest_child;
        
    }

    private Vector3 culc_avoid_obstacle(){
        Vector3 accel = Vector3.zero;
        int count = 0;
        while(count <= 1000){
            Vector3 dir = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            Vector3 update_dir = Quaternion.Euler(dir[0], dir[1], dir[2]) * this.GetComponent<Rigidbody>().velocity.normalized;
            Ray new_ray = new Ray(this.transform.position, update_dir * ray_long);
            Debug.DrawRay(this.transform.position, update_dir * ray_long, Color.red);
        
            RaycastHit new_hit;
            //rayをが当たらなかった方向へ進む
            if(!(Physics.Raycast(new_ray, out new_hit, ray_long))){
                    accel += update_dir.normalized * avoid_obst_weight;
                    //Debug.Log("レイキャスト反応");
                    return accel;
            }
            count++;
        }
        return accel;
    }
}


    
/*
    private Vector3 keep_dist(){ //一番最初に見つけた任意の距離より近いオブジェクトから離れようとするベクトルを返す,任意の距離より近いオブジェクトがないときは0ベクトルを返す
        children = Manager_script.children;
        foreach(GameObject child in children){
            if(this.transform.parent.gameObject == child){
                continue;
            }
        
            //任意のchild同士の距離を計算する
            Transform child_t = child.transform.GetChild(0);
            GameObject child_ob = child_t.gameObject;
            var diff = this.transform.position - child_ob.transform.position;
            //任意の距離よりオブジェクト間の距離が小さい場合, 反対方向へ進むようにする
            if(diff.magnitude < keep_distance + Random.Range(-0.5f, 0.5f)){
                Debug.Log("離れる");
                //現在の速度に他のオブジェクトから離れる方への速度を足し合わせる
                return diff.normalized;                
            }            
        }
        //任意の距離より近いオブジェクトがないときは0ベクトルを返す
        return Vector3.zero; //重心の方を向いた大きさ１のベクトルを返す
    }

    private Vector3 ahead_ave(){
        ave_velocity = Manager_script.ave_velocity;
        //現在の速度に全体平均の速度を足し合わせる
        return ave_velocity; //群れの平均の移動方向を向いた大きさ１のベクトルを返す
    }
    */

