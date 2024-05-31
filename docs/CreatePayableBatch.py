import os
import json
import uuid
import psycopg2
from datetime import datetime, timedelta

conn = psycopg2.connect(
    dbname="receivables-flow",
    user="postgres",
    password="postgres",
    host="localhost",
    port="5432"
)

if conn.closed:
    print("Connection is closed.")
else:
    print("Connection is open.")

cur = conn.cursor()

if cur is None:
    print("Cursor creation failed.")
else:
    print("Cursor created successfully.")

cur.execute('SELECT "Id" FROM "Assignors" LIMIT 1;')

assignorid = cur.fetchone()

cur.close()
conn.close()

print(assignorid)

data = []
for i in range(10000):
    data.append({
        "Value": 100.0 + i,
        "EmissionDate": (datetime.now() + timedelta(days=i)).isoformat(),
        "AssignorId": assignorid[0],
    })

# Check if the directory exists
if not os.path.exists('batch'):
    # If not, create the directory
    os.makedirs('batch')

# Now you can write to the file in the 'batch' directory
with open('batch/payables_batch.json', 'w') as f:
    json.dump(data, f)
