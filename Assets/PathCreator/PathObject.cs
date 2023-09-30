using UnityEngine;
using PathCreation;

public class PathObject : MonoBehaviour
{
    [SerializeField] PathCreator pathCreator;
    [SerializeField] EndOfPathInstruction endOfPath;
    [SerializeField] float distanceOnPath;
    [SerializeField] float moveSpeed;

    void OnValidate()
    {
        if (pathCreator == null) return;

        //Set the distanceOnPath to loop to the begining of the path to avoid it reaching over the maximum float value
        distanceOnPath = Mathf.Repeat(distanceOnPath, pathCreator.path.length * 2.0f);
        
        //Set the platform position to the path
        transform.position = pathCreator.path.GetPointAtDistance(distanceOnPath, endOfPath);
    }

    void Update()
    {
        //Move the distance the platform is on the path
        distanceOnPath += Time.deltaTime * moveSpeed;

        //Set the distanceOnPath to loop to the begining of the path to avoid it reaching over the maximum float value
        distanceOnPath = Mathf.Repeat(distanceOnPath, pathCreator.path.length * 2.0f);

        //Set the platform position to the path
        transform.position = pathCreator.path.GetPointAtDistance(distanceOnPath, endOfPath);
    }
}
