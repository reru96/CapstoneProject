using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Transform[] cameraPositions;
    public Transform[] cameraTargets;
    public float moveSpeed = 2f;
    public float rotateSpeed = 2f;

    private int currentIndex = 0;
    private bool isMoving = true; 
    private float stopDistance = 0.05f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextCharacter();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PreviousCharacter();
        }

        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            cameraPositions[currentIndex].position,
            Time.deltaTime * moveSpeed
        );

        Quaternion lookRotation = Quaternion.LookRotation(cameraTargets[currentIndex].position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);

        if (Vector3.Distance(transform.position, cameraPositions[currentIndex].position) < stopDistance)
            isMoving = false;
    }

    public void NextCharacter()
    {
        currentIndex++;
        if (currentIndex >= cameraPositions.Length)
            currentIndex = 0;
        isMoving = true;
    }

    public void PreviousCharacter()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = cameraPositions.Length - 1;
        isMoving = true;
    }
}
