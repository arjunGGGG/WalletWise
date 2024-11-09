from flask import Flask, request, jsonify
import pandas as pd

app = Flask(__name__)

@app.route('/get_value', methods=['POST'])
def get_value():
    try:
        data = request.get_json()
        if not data or 'index' not in data:
            return jsonify({'error': 'No index provided'}), 400
        
        index = int(data['index'])
        
        df = pd.read_csv('model/submission.csv')
        
        value = float(df.iloc[index, 2])
        
        return jsonify({'value': value})
    
    except IndexError:
        return jsonify({'error': 'Index out of range'}), 400
    except ValueError:
        return jsonify({'error': 'Invalid index or value cannot be converted to float'}), 400
    except FileNotFoundError:
        return jsonify({'error': 'CSV file not found'}), 500
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=11436)
