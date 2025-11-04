import dspy

# Define a simple DSPy "predictor"
class MyPredictor(dspy.Module):
    def __init__(self):
        super().__init__()
        self.predict = dspy.Predict("input -> output")

    def forward(self, input):
        return self.predict(input=input)

# Instantiate the LM (change model name if you want)
dspy.configure(lm=dspy.OpenAI(model="gpt-4o-mini"))

# Run the predictor
predictor = MyPredictor()
result = predictor("Hello DSPy!")
print(result.output)
