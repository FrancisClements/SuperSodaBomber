using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Projectile ScriptableObject", menuName = "ScriptableObjects/Projectile")]
public class Projectile_ScriptObject : ScriptableObject
{
    //selected type
    public ProjectileTypes projectileType;

    //throwing physics
    public float throwX = 3f; 
    public float throwY = 250f;
    public float spin = 200f;

    public bool gravity = true;

    //if it uses splash damage and explosion fx
    public bool isExplosive = false;
    public GameObject explosionPrefab;
    public int explosionAmount = 1;

    public bool isSplashDamage = false;
    public float blastRadius;

    public ExplosionType explosionType;
    public float detonateTime = 2.5f;

    public bool applyMovingMechanic = true;
    public float throwingMultiplier = 2.5f;

    #if UNITY_EDITOR
    void OnEnable() => EditorUtility.SetDirty(this);
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Projectile_ScriptObject))]
public class Projectile_ScriptObject_Editor: Editor{
    private void AddIndent(){
        EditorGUI.indentLevel++;
    }

    private void RemoveIndent(){
        EditorGUI.indentLevel--;
    }

    public override void OnInspectorGUI()
    {
        var s = target as Projectile_ScriptObject;

        //used to show the sprite list at the custom editor
        SerializedObject serialObj = new SerializedObject(target);
        SerializedProperty sprites = serialObj.FindProperty("spriteList");

        //Data Input
        s.projectileType = (ProjectileTypes)EditorGUILayout.EnumPopup("Projectile Type", s.projectileType);

        //Explosion Properties
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Explosion Properties", EditorStyles.boldLabel);

        //Enable Splash Damage
        s.isExplosive = EditorGUILayout.Toggle("Enable FX", s.isExplosive);

        if (s.isExplosive){
            AddIndent();
            s.explosionPrefab = (GameObject)EditorGUILayout.ObjectField("Explosion Prefab", s.explosionPrefab, typeof(GameObject), false);
            s.explosionAmount = EditorGUILayout.IntSlider("Spawning Amount", s.explosionAmount, 1, 10);
            RemoveIndent();
        }

        s.isSplashDamage = EditorGUILayout.Toggle("Enable Splash Damage", s.isSplashDamage);

        if (s.isSplashDamage){
            AddIndent();
            s.blastRadius = EditorGUILayout.Slider("Blast Radius", s.blastRadius, 1f, 3f);
            RemoveIndent();
        }

        //Explosion Type
        s.explosionType = (ExplosionType)EditorGUILayout.EnumPopup("Explosion Type", s.explosionType);

        if (s.explosionType == ExplosionType.Detonate || s.explosionType == ExplosionType.Delay){
            AddIndent();
            s.detonateTime = EditorGUILayout.Slider("Detonation Time", s.detonateTime, 0.5f, 5f);
            
            RemoveIndent();
        }

        //Physics
        if(s.explosionType != ExplosionType.Instant){
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Physics", EditorStyles.boldLabel);

            s.throwX = EditorGUILayout.FloatField("Throw X", s.throwX);
            s.throwY = EditorGUILayout.FloatField("Throw Y", s.throwY);
            s.spin = EditorGUILayout.FloatField("Spin", s.spin);
            s.gravity = EditorGUILayout.Toggle("Gravity", s.gravity);

            EditorGUILayout.Space();
            
            //Moving Mechanic
            s.applyMovingMechanic = EditorGUILayout.Toggle("Apply Moving Mechanic", s.applyMovingMechanic);
            using(new EditorGUI.DisabledScope(!s.applyMovingMechanic)){
                s.throwingMultiplier = EditorGUILayout.Slider("Throwing Multiplier", s.throwingMultiplier, 1f, 3f);
            }
        }
    }
}
#endif