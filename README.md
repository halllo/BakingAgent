# Baking Agent

This experimental application explores local agentic capabilities in a christmas theme.

Run the OCR.

```bash
ollama run deepseek-ocr "/Users/manuelnaujoks/Projects/BakingAgent/recipe.jpg\n<|grounding|>Convert the document to markdown."
```

Run the LLM.

```bash
ollama run qwen3-vl:4b-instruct
```

Run the agent backend.

```bash
dotnet run --project apphost
```

Run the agent frontend.

```bash
cd baking.cli && dotnet run agent "Take a look at this recipe: /Users/manuelnaujoks/Projects/BakingAgent/recipe.jpg Make sure all ingredients are available and bake it."
```
