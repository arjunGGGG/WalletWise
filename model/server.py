from flask import Flask, request, jsonify
import pandas as pd

app = Flask(__name__)

@app.route('/get_value', methods=['POST'])
def get_value():
    try:
        # Get the index from request
        data = request.get_json()
        if not data or 'index' not in data:
            return jsonify({'error': 'No index provided'}), 400
        
        index = int(data['index'])
        
        # Read the CSV file
        df = pd.read_csv('model/submission.csv')
        
        # Get the value from the third column at the specified index
        value = float(df.iloc[index, 2])  # 2 represents the third column (0-based indexing)
        
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