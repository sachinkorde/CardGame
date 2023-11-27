using UnityEngine;

public class ExamplesInUnity : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public Vector3 translation = new(0.1f, 0.0f, 0.0f);
    public Vector3 scale = new(1.1f, 1.1f, 1.1f);

    void Update()
    {
        //Matrix Examples

        //Rotation
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(transform.rotation);
        float angleOfRotation = rotationSpeed * Time.deltaTime;
        Matrix4x4 rotationInY = Matrix4x4.Rotate(Quaternion.Euler(0, angleOfRotation, 0));
        Matrix4x4 newRotationMatrix = rotationInY * rotationMatrix;
        transform.rotation = newRotationMatrix.rotation;

        //Translation
        Matrix4x4 translationMatrix = Matrix4x4.Translate(translation);
        transform.position = translationMatrix.MultiplyPoint(transform.position);

        //Scaling   
        Matrix4x4 scalingMatrix = Matrix4x4.Scale(scale);
        transform.localScale = scalingMatrix.MultiplyVector(scale);

        //Matrix4x4.Determinant 
    }
}
