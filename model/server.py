from flask import Flask, jsonify

app = Flask(__name__)

@app.route('/get_integer', methods=['GET'])
def get_integer():
    response_integer = 42  # You can change this integer to whatever you need
    return jsonify(response_integer)

if __name__ == '__main__':
    app.run(host='127.0.0.1', port=5000)
