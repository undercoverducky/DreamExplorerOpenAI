curl https://api.openai.com/v1/images/generations \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer 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" \
  -d '{
    "prompt": "River surface",
    "n": 1,
    "size": "256x256"
  }'
