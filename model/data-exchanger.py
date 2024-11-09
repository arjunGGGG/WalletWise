from flask import Flask, request, jsonify

app = Flask(__name__)

# Predefined dictionary data (response)
product_prices = {
    "smartphone": 30000,
    "laptop": 70000,
    "earbuds": 5000,
    "smartwatch": 10000,
    "washing machine": 40000,
    "refrigerator": 50000,
    "AC": 50000,
    "oven": 15000,
    "casual-wear": 2000,
    "formal-wear": 5000,
    "luxury-bag": 50000,
    "skincare": 5000,
    "gym-membership": 40000,
    "supplements": 40000,
    "netflix": 10000,
    "video-game": 5000,
    "domestic-flight": 10000,
    "luxury-car": 7500000,
    "bike": 300000,
    "scooter": 100000,
    "luxury-watch": 75000
}

@app.route('/', methods=['POST'])
def process_data():
    try:
        # Get the incoming JSON data
        data = request.get_json()

        # Check if data was received
        if not data:
            return jsonify(error="No data received."), 400

        # Print the received data for debugging
        print("Received data:", data)

        # Return the product prices dictionary
        return jsonify(product_prices)

    except Exception as e:
        print("Error processing data:", e)
        return jsonify(error="An error occurred. Please check the server logs."), 500

if __name__ == '__main__':
    app.run(port=11435, debug=True)
