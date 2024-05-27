import json
import uuid
from datetime import datetime, timedelta

data = []
for i in range(10000):
    data.append({
        "Value": 100.0 + i,
        "EmissionDate": (datetime.now() + timedelta(days=i)).isoformat(),
        "AssignorId": str(uuid.uuid4())
    })

with open('payables_batch.json', 'w') as f:
    json.dump(data, f)