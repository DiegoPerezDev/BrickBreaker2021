using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MyTools
{
    public class SearchTools
    {
        /*
        * - - - NOTES - - -
        - This class is just for finding gameObjects and components evading errors if they are not found. 
        - All of these methods happen in the awake of the monoBehaviour or in the setting functions of the transition of the scenes.
        */


        /// <summary>
        /// Try to find a gameObject with a direction path of the Unitys hierarchy. If not, then it stops the game and tells us the path in which we tried to find the gameObject.
        /// </summary>
        /// <param name="gameobjectPath"> String path of the gameObject in the Unitys Hierarchy.</param>
        public static GameObject TryFind(string gameobjectPath)
        {
            GameObject desiredGameobject = GameObject.Find(gameobjectPath);
            if (desiredGameobject == null)
            {
                Debug.Log($"Could not find the gameObject in the path: {gameobjectPath}.");
                Debug.Break();
            }
            return desiredGameobject;
        }

        /// <summary>
        /// Try to find a gameObject inside another gameObject via transform.Find(). If not found, then it stops the game and tells us the path in which we tried to find the gameObject.
        /// </summary>'
        /// <param name="gameobjectPath"> String path of the child. This path begins after the name of the parent and ends with the name of the child we are trying to find.</param>
        /// <returns></returns>
        public static GameObject TryFindInGameobject(GameObject parent, string gameobjectPath)
        {
            //Look if the parent gameObject exist.
            if (parent == null)
            {
                Debug.Log($"No gameObject found with the name: {nameof(parent)}.");
                Debug.Break();
                return null;
            }

            //Look if the child gameObject exist and the path is correct.
            Transform childTransform = parent.transform.Find(gameobjectPath);
            if (childTransform != null)
            {
                return childTransform.gameObject;
            }
            else
            {
                Debug.Log($"Could not find the gameObject in the path: {nameof(parent)}/{gameobjectPath}.");
                Debug.Break();
                return null;
            }
        }

        /// <summary>
        /// Try to get a component from a gameObject and return it. If not, then it stops the game and tells us the parent in which we tried to find the component.
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="gameobject"> The gameObject that contains the component we are looking for.</param>
        public static T TryGetComponent<T>(GameObject gameobject)
        {
            //Look if the gameObject exist.
            if (gameobject == null)
            {
                Debug.Log($"No gameObject found with the name: {nameof(gameobject)}.");
                Debug.Break();
                return default;
            }

            //Look if the gameObject have the desired component.
            T temp;
            try
            {
                temp = gameobject.GetComponent<T>();
            }
            catch
            {
                Debug.Log($"Could not find the component '{typeof(T)}' in the gameObject: {nameof(gameobject)}.");
                Debug.Break();
                return default;
            }
            
            return gameobject.GetComponent<T>();
        }

        /// <summary>
        /// Try to load a resourse of the project on a given path.
        /// </summary>
        /// <param name="path">Path of the file to get to that resource.</param>
        /// <returns></returns>
        public static UnityEngine.Object TryLoadResource(string path)
        {
            UnityEngine.Object resource = Resources.Load(path);
            if(resource == null)
            {
                Debug.Log($"Couldn't load resource in path: '{path}'.");
                return null;
            }
            return resource;
        }
    }

}