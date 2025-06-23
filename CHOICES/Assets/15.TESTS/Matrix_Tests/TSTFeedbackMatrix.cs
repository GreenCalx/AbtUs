using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class TSTFeedbackMatrix
{
    public int maxMatrixSize = 10;
    // A Test behaves as an ordinary method
    [Test]
    public void TSTFeedbackMatrixSimplePasses()
    {
        // Build feedback list
        FeedbackData fData1 = new FeedbackData();
        fData1.tag = (OWCAxis)0;
        fData1.type = FeedbackType.obj_nature;
        fData1.baseValue = 1f;
        fData1.loopStrength = 0f;

        GameObject gof1 = new GameObject();
        GameFeedback f1 = gof1.AddComponent(typeof(GameFeedback)) as GameFeedback;
        f1.fData = fData1;

        List<GameFeedback> gameFeedbacks = new List<GameFeedback>();
        gameFeedbacks.Add(f1);

        for (int i = 0; i < maxMatrixSize; i++)
        {
            // test build
            FeedbackMatrix fMatrix = new FeedbackMatrix(i);
            Assert.That(fMatrix != null);
            Assert.That(fMatrix.feedbacks.Count == i);
            Assert.That(fMatrix.outputs.Keys.Count == i);

            // Test Add/Remove feedback in matrix
            foreach (var gf in gameFeedbacks)
            {
                Feedback f = fMatrix.BuildFeedback(gf);
                Assert.That(f != null);

                fMatrix.AddFeedback(f);
                if (i == 0)
                { Assert.That(fMatrix.feedbacks.Count == 0); }
                else
                { Assert.That(fMatrix.feedbacks[(int)fData1.tag].Count == 1); }

                fMatrix.RemoveFeedback(f);
                if (i == 0)
                { Assert.That(fMatrix.feedbacks.Count == 0); }
                else
                { Assert.That(fMatrix.feedbacks[(int)fData1.tag].Count == 0); }
            }

            // Test matrix outputs
            // TODO
        }
    }

    [UnityTest]
    public IEnumerator TSTFeedbackMatrix_Inertia()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TSTFeedbackMatrixWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
