# Running tests

## Test Runs

Test runs is a collection of test cases for execution. They have states, results and other fields related to the findings of test execution.

## How to run tests?

### Creating a new test run from the test tree view

A new test run can be created by right-clicking on a test suite, a folder, or a test case and selecting "Run".

### Importing test runs

Automated tests that produce a result file (Junit XML, xUnit XML, .TRX, etc) can be imported with results. 
Importing  a test run will also create test suites and test cases if they are missing.

## Timings

The duration to execute a test case is recorded and measured from when testing starts, until the result is set.

## Adding attachments

Select the test case and click on the "Attachments" tab to the right.

Attachments can be added either by drag-and-dropping files to the designated drop zone or by clicking in the drop zone and browsing for a file.

## Adding desktop screenshots

Not supported. Should be implemented by javascript APIs.

```javascript
const capture = async () => {
  const canvas = document.createElement("canvas");
  const context = canvas.getContext("2d");
  const video = document.createElement("video");

  try {
    const captureStream = await navigator.mediaDevices.getDisplayMedia();
    video.srcObject = captureStream;
    context.drawImage(video, 0, 0, window.width, window.height);
    const frame = canvas.toDataURL("image/png");
    captureStream.getTracks().forEach(track => track.stop());
    window.location.href = frame;
  } catch (err) {
    console.error("Error: " + err);
  }
};
```

