using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class Tools : MonoBehaviour
{

    #region Color
    public static Color HexToColor(string hex)
    {
        // Remove the '#' if present
        hex = hex.Replace("#", "");

        // Convert the hexadecimal color code to RGB values
        float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

        // Create and return the Color object
        return new Color(r, g, b);
    }

    public static Color Darken(Color original, float darkenFactor = .9f) // 0-1
    {
        //    float darkenFactor = 0.9f; // 10% darker
        float r = original.r * darkenFactor;
        float g = original.g * darkenFactor;
        float b = original.b * darkenFactor;

        return new Color(r, g, b, original.a);
    }
    #endregion

    #region LoadingSavingImages

    public static string ServerPathLocation;
    public static string PersistantDataPath = Application.persistentDataPath + "/";
    public static IEnumerator LoadImageFromServerURL(string imagePath, Image cell)
    {



        Sprite icon = m_loadImage(PersistantDataPath + imagePath);
        if (icon != null)
        {
            cell.sprite = icon;
            yield break;
        }

        cell.DOFade(0, 0);

        Tools.Log("URL Path: " + ConstructURL(ServerPathLocation, imagePath));
        // Check internet connection
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return null;
        }


        UnityWebRequest www = UnityWebRequestTexture.GetTexture(ConstructURL(ServerPathLocation, imagePath));
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.ProtocolError && www.result != UnityWebRequest.Result.ConnectionError && www.downloadHandler.isDone)
        {

            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            cell.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            cell.DOFade(1, 0.2f);
            m_saveImage(PersistantDataPath + imagePath, loadedTexture.EncodeToPNG());
        }

    }
    public static IEnumerator LoadImageFromURL(string Path, Image cell, UnityAction OnComplete = null)
    {

        cell.DOFade(0, 0);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(Path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.ProtocolError && www.result != UnityWebRequest.Result.ConnectionError && www.downloadHandler.isDone)
        {

            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            cell.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            cell.DOFade(1, 0.2f).OnStart(() => OnComplete?.Invoke());

        }

    }
    public static IEnumerator LoadSpriteFromURL(string Path, Action<Sprite> OnSucces)
    {

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(Path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.ProtocolError && www.result != UnityWebRequest.Result.ConnectionError && www.downloadHandler.isDone)
        {

            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);

            Sprite e = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            OnSucces(e);
        }

    }


    public static Sprite LoadSpriteResources(string Path)
    {

        return Resources.Load<Sprite>(Path);
    }


    // Private
    //this construction of url is to download from Net.
    private static string ConstructURL(string serverPath, string imagePath)
    {
        return serverPath + imagePath + ".png";
    }
    static void m_saveImage(string path, byte[] imageBytes)
    {

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path + ".png", imageBytes);
            Tools.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Tools.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Tools.LogWarning("Error: " + e.Message);
        }
    }
    static Sprite m_loadImage(string path)
    {
        byte[] dataByte = null;
        string fullPath = path + ".png";
        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Tools.LogWarning("Directory does not exist " + path);
            return null;
        }

        if (!File.Exists(fullPath))
        {
            Tools.Log("File does not exist");
            return null;
        }

        try
        {
            dataByte = File.ReadAllBytes(fullPath);
            Tools.Log("Loaded Data from: " + fullPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Tools.LogWarning("Failed To Load Data from: " + fullPath.Replace("/", "\\"));
            Tools.LogWarning("Error: " + e.Message);
        }

        Texture2D tex = m_convertBytesToTexture(dataByte);


        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
    static Texture2D m_convertBytesToTexture(byte[] LoadedImage)
    {
        Texture2D texture;
        texture = new Texture2D(2, 2);
        texture.LoadImage(LoadedImage);
        return texture;
    }
    #endregion

    #region String Convertion
    public static string ShortNumeric(float Score)
    {
        string result;
        string[] ScoreNames = new string[] { "", "K", "M", "B", "T", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz" };
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Score < 900)
                break;
            else Score = Mathf.Floor(Score / 100f) / 10f;

        if (Score == Mathf.Floor(Score))
            result = Score.ToString() + ScoreNames[i];
        else result = Score.ToString("F1") + ScoreNames[i].ToUpper();
        return result;
    }
    #endregion

    #region Chance Convertion

    [Tooltip("Return a random weighted index from Array list, Example:  You need to put all your chances in array. and it will return the random index of this array")]
    public static int GetRandomValue(int[] Chances)
    {
        int totalChances = 0;
        foreach (int item in Chances)
        {
            totalChances += item;
        }

        int randomValue = Random.Range(0, totalChances);
        int accumulatedChance = 0;

        for (int i = 0; i < Chances.Length; i++)
        {
            accumulatedChance += Chances[i];
            if (randomValue < accumulatedChance)
            {
                return i;
            }

        }

        return 0;
    }
    #endregion

    #region DebugDevelop
    public static void Log(string v, Color color = default)
    {

#if UNITY_EDITOR
        if (color != default)
        {
            // v = v.Replace("$", "");
            Debug.Log("<color=black>[Tools]</color> " + $"<color=#{ColorUtility.ToHtmlStringRGB(color)}> {v}</color>");
        }
        else
        {

            Debug.Log("<color=black>[Tools]</color> " + v);
        }

#endif
    }
    public static void LogWarning(string v)
    {
#if UNITY_EDITOR
        Debug.LogWarning("<color=orange>[Tools]</color> " + v);
#endif
    }

    public static void LogError(string v)
    {
#if UNITY_EDITOR
        Debug.LogError("<color=red>[Tools]</color> " + v);
#endif
    }
    #endregion

    #region Screen

    public static float ReturnScreenFitScaleMultiplier(SpriteRenderer spriteRenderer, float targetWidthPercentage = 0.8f, float defaultOrthoSize = 5, bool width = true)
    {


        float scale = 1;

        // Get the aspect ratio of the screen
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        // Calculate the orthographic size needed for the sprite width
        float orthographicSizeNeeded = width ? spriteRenderer.bounds.size.x : spriteRenderer.bounds.size.y / (2 * aspectRatio);

        float orthographicTargetSizeNeeded = orthographicSizeNeeded / targetWidthPercentage;

        //if (orthographicSizeNeeded < defaultOrthoSize)
        //{

        //    scale = 1;
        //}
        //else {
        //    scale = (orthographicSizeNeeded / defaultOrthoSize);
        //}
        scale = (orthographicSizeNeeded / (defaultOrthoSize * targetWidthPercentage));

        return scale;

    }


    public static float ReturnScreenFitScaleMultiplier(List<Collider> colliders, float targetWidthPercentage = 0.8f, float defaultOrthoSize = 5, bool width = true)
    {
        float scale = 1;

        // Check if the colliders list is empty
        if (colliders == null || colliders.Count == 0)
        {
            Debug.LogError("Colliders list is empty.");
            return scale;
        }

        // Compute combined bounds of all colliders
        Bounds combinedBounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Count; i++)
        {
            combinedBounds.Encapsulate(colliders[i].bounds);
        }

        // Get the aspect ratio of the screen
        float aspectRatio = (float)Screen.width / (float)Screen.height;

        // Calculate the orthographic size needed for the combined colliders
        float orthographicSizeNeeded;
        if (width)
        {
            orthographicSizeNeeded = combinedBounds.size.x / 2; // Divide by 2 because orthographic size is half the height
            orthographicSizeNeeded /= aspectRatio; // Adjust for screen aspect ratio
        }
        else
        {
            orthographicSizeNeeded = combinedBounds.size.y / 2;
        }

        // Adjust for the target width or height percentage
        orthographicSizeNeeded /= targetWidthPercentage;

        // Compute the scale based on the target orthographic size and default orthographic size
        scale = orthographicSizeNeeded / defaultOrthoSize;

        return scale;
    }
    public static float ReturnScreenFitScaleMultiplier(Bounds bounds, float targetWidthPercentage = 0.8f, float defaultOrthoSize = 5, bool width = true, float fixedRatio = -1)
    {
        float scale = 1;

        // Get the aspect ratio of the screen
        float aspectRatio = fixedRatio > 0 ? fixedRatio : (float)Screen.width / (float)Screen.height;



        // Calculate the orthographic size needed for the bounds width or height
        float orthographicSizeNeeded;
        if (width)
        {
            orthographicSizeNeeded = bounds.size.x / 2; // Divide by 2 because orthographic size is half the height
            orthographicSizeNeeded /= aspectRatio; // Adjust for screen aspect ratio
        }
        else
        {
            orthographicSizeNeeded = bounds.size.y / 2;
        }

        // Adjust for the target width or height percentage
        orthographicSizeNeeded /= targetWidthPercentage;

        // Compute the scale based on the target orthographic size and default orthographic size
        scale = orthographicSizeNeeded / defaultOrthoSize;

        return scale;
    }
    public static float GetScreenOrthographicWidth()
    {
        return Camera.main.orthographicSize * 2 * Screen.width / Screen.height;
    }

    public static Vector3 GetPointOnScreenEdge(Vector3 direction)
    {
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        Vector3 screenTop = Camera.main.transform.position + Vector3.up * cameraHeight;
        Vector3 screenBottom = Camera.main.transform.position + Vector3.down * cameraHeight;
        Vector3 screenLeft = Camera.main.transform.position + Vector3.left * cameraWidth;
        Vector3 screenRight = Camera.main.transform.position + Vector3.right * cameraWidth;

        Vector3 directionNormalized = direction.normalized;

        Vector3 cameraPos = Camera.main.transform.position;

        // Calculate t for each edge intersection
        float tTop = (screenTop.y - cameraPos.y) / direction.y;
        float tBottom = (screenBottom.y - cameraPos.y) / direction.y;
        float tLeft = (screenLeft.x - cameraPos.x) / direction.x;
        float tRight = (screenRight.x - cameraPos.x) / direction.x;

        // Find the smallest positive t (since we want to project in the given direction)
        float t = Mathf.Min(Mathf.Max(tTop, 0), Mathf.Max(tBottom, 0), Mathf.Max(tLeft, 0), Mathf.Max(tRight, 0));

        // Get the point on the screen edge by scaling the direction vector
        Vector3 edgePoint = cameraPos + direction * t;

        return edgePoint;

    }

    #endregion

    #region Collider Tools
    public static Bounds GetCombinedBoundingBoxOfChildren(Transform root)
    {
        if (root == null)
        {
            throw new ArgumentException("The supplied transform was null");
        }

        var colliders = root.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            throw new ArgumentException("The supplied transform " + root?.name + " does not have any children with colliders");
        }

        Bounds totalBBox = colliders[0].bounds;
        foreach (var collider in colliders)
        {
            totalBBox.Encapsulate(collider.bounds);
        }
        return totalBBox;
    }
    #endregion

    #region Array
    public static int[] GetRandomArray(int wantedArrayLength, int arrayMaxLength, params int[] ExclueNumbers)
    {

        List<int> l = new List<int>();
        int[] shuffeldAarray = new int[wantedArrayLength];

        for (int i = 0; i < arrayMaxLength; i++)
        {
            bool excluded = false;
            foreach (var item in ExclueNumbers)
            {
                if (item == i)
                {
                    excluded = true;
                    break;
                }
            }

            if (!excluded) l.Add(i);
        }

        for (int i = 0; i < wantedArrayLength; i++)
        {
            int rand = Random.Range(0, l.Count);
            shuffeldAarray[i] = l[rand];
            // Debug.Log(shuffeldAarray[i]);
            l.RemoveAt(rand);
        }

        l.Clear();
        return shuffeldAarray;
    }
    #endregion



    #region Find Target

    public static float CalculatePathDistance(Vector3[] pathPoints)
    {
        float totalDistance = 0f;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            totalDistance += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }

        return totalDistance;
    }
    public static Transform FindClosestTransform(Transform target, List<Transform> transforms)
    {
        if (transforms == null || transforms.Count == 0)
        {
            return null;
        }

        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform t in transforms)
        {
            // Skip the target itself
            if (t == target)
            {
                continue;
            }

            float distance = Vector3.Distance(target.position, t.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = t;
            }
        }

        return closest;
    }
    // Method to find the center (centroid) of a list of Vector3 points
    public static Vector3 CalculateCenterPoint(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
        {
            Debug.LogError("The list of points is empty or null.");
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;

        // Sum up all the points
        foreach (Vector3 point in points)
        {
            sum += point;
        }

        // Divide by the number of points to get the average (center point)
        return sum / points.Count;
    }

    // Function to check if a point is inside a polygon in the XZ plane
    public static bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        int numIntersections = 0;
        int count = polygon.Count;

        for (int i = 0; i < count; i++)
        {
            Vector3 vertex1 = polygon[i];
            Vector3 vertex2 = polygon[(i + 1) % count];

            // Check if the point is on an edge (optional, but useful)
            if (IsPointOnLine(point, vertex1, vertex2))
                return true;

            // Check if the point crosses the edge from left to right in the XZ plane
            if ((vertex1.z > point.z) != (vertex2.z > point.z))
            {
                float xIntersection = (point.z - vertex1.z) * (vertex2.x - vertex1.x) / (vertex2.z - vertex1.z) + vertex1.x;
                if (point.x < xIntersection)
                {
                    numIntersections++;
                }
            }
        }

        // If number of intersections is odd, the point is inside
        return (numIntersections % 2 != 0);
    }
    // Helper function to check if a point is on a line segment in the XZ plane
    private static bool IsPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        // Cross product to check collinearity in the XZ plane
        float crossProduct = (point.z - lineStart.z) * (lineEnd.x - lineStart.x) - (point.x - lineStart.x) * (lineEnd.z - lineStart.z);
        if (Mathf.Abs(crossProduct) > Mathf.Epsilon)
            return false;

        // Dot product to check if point is within the line segment
        float dotProduct = (point.x - lineStart.x) * (lineEnd.x - lineStart.x) + (point.z - lineStart.z) * (lineEnd.z - lineStart.z);
        if (dotProduct < 0)
            return false;

        float squaredLength = (lineEnd.x - lineStart.x) * (lineEnd.x - lineStart.x) + (lineEnd.z - lineStart.z) * (lineEnd.z - lineStart.z);
        return dotProduct <= squaredLength;
    }

    #endregion

    #region Mouse
    public static bool IsPointerOverUIObject(params GameObject[] UIObjects)
    {
        // Check if the pointer is over a UI object
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };

        // Check all UI raycast results
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Check if the clicked object is one of the specified buttons
        foreach (RaycastResult result in results)
        {
            foreach (GameObject obj in UIObjects)
            {
                if (result.gameObject == obj)
                {
                    return true; // Pointer is over one of the buttons
                }
            }


        }
        return false;
    }
    #endregion

    #region Grid
    public enum Direction
    {
        LEFT, RIGHT, TOP, BOTTOM, DIAGONAL
    }
    public static Direction GetLookDirection(Vector2 from, Vector2 to)
    {
        Vector2 delta = to - from;

        if (delta == Vector2.up) return Direction.TOP;
        if (delta == Vector2.down) return Direction.BOTTOM;
        if (delta == Vector2.left) return Direction.LEFT;
        if (delta == Vector2.right) return Direction.RIGHT;

        return Direction.DIAGONAL;
    }
    #endregion

    #region UI
    public static Vector2 GetRectPost(RectTransform targetRect)
    {
        RectTransform targetParent = targetRect.parent as RectTransform;
        Canvas canvas = targetRect.GetComponentInParent<Canvas>();
        Camera cam = canvas.worldCamera;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, targetRect.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetParent,
            screenPoint,
            cam,
        out localPoint

            );

        Vector2 result = localPoint; // this is the anchoredPosition you can use

        return result;
    }
    #endregion
    #region Positions
    public static Vector3 CenterOfMass(List<Vector3> positionList)
    {
        if (positionList.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;

        // Calculate the center of mass (average position)
        foreach (Vector3 pos in positionList)
        {
            center += pos;
        }
        center /= positionList.Count;

        return center;
    }
    #endregion
}
