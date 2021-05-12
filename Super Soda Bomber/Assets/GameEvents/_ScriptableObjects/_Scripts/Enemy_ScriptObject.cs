using UnityEngine;
using UnityEditor;
using System;

/*
Enemy_ScriptObject
    A scriptable object that holds the initial data for the enemies:
        - Shooter
        - Roller
*/
namespace SuperSodaBomber.Enemies{
    [CreateAssetMenu(fileName = "New Enemy ScriptableObject", menuName = "ScriptableObjects/Enemy")]
    public class Enemy_ScriptObject : ScriptableObject
    {
        public float health = 100f;
        public float movementSpeed = 0;
        public float attackRate = 1.5f;

        //attacking range for the enemy
        public float attackRadius = 1.5f;
        //range where the enemy can look at the enemy
        public float spotRadius = 3f;

        public GameObject projectilePrefab;
        public EnemyType enemyType;
        public bool isMoving = false;
        public bool facingRight = true;

        public float healthMultiplier = 1f;
        public float attackRateMultiplier = 1f;

        #if UNITY_EDITOR
        void OnEnable() => EditorUtility.SetDirty(this);
        #endif

    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(Enemy_ScriptObject))]
    public class Enemy_ScriptObject_Editor: Editor{
        public override void OnInspectorGUI()
        {
            var s = target as Enemy_ScriptObject;

            //Data Input
            s.health = EditorGUILayout.Slider("Health", s.health, 50f, 500f);
            s.movementSpeed = EditorGUILayout.Slider("Movement Speed", s.movementSpeed, 0, 3f);
            s.attackRate = EditorGUILayout.Slider(new GUIContent("Attack Rate", "Lower = faster"), s.attackRate, .01f, 5f);

            s.attackRadius = EditorGUILayout.Slider("Attack Radius", s.attackRadius, .5f, 10f);
            s.spotRadius = EditorGUILayout.Slider("Spot Radius", s.spotRadius, 1f, 15f);


            s.projectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", s.projectilePrefab, typeof(GameObject), false);
            s.enemyType = (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", s.enemyType);
            s.isMoving = EditorGUILayout.Toggle("Is Moving", s.isMoving);
            s.facingRight = EditorGUILayout.Toggle("Facing Right", s.facingRight);

            //Phase 2
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Phase 2 Modifiers", EditorStyles.boldLabel);
            s.healthMultiplier = EditorGUILayout.Slider("Health Multiplier", s.healthMultiplier, 1f, 3f);
            s.attackRateMultiplier = EditorGUILayout.Slider("Attack Rate Multiplier", s.attackRateMultiplier, 1f, 2f);
        }
    }
    #endif
}
