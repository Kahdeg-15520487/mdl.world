# Fantasy World Generator API - Test Suite

This directory contains comprehensive test scripts for the Fantasy World Generator API. The test suite validates all major functionality including world generation, enhancement, storage, and LLM integration.

## 📁 Test Directory Structure

```
test/
├── TEST-README.md                    # This documentation
├── test-api-suite.ps1               # Main comprehensive test suite
├── test-simple.ps1                  # Simple PowerShell test script
├── test-quick.bat                   # Windows batch quick tests
├── test-quick.sh                    # Unix/Linux/macOS shell quick tests
├── test-api-collection.http         # HTTP test collection for VS Code
├── test-request.json                # Sample world generation request
├── test-enhance-request.json        # Sample enhancement request
├── test-llm-generation.http         # LLM integration tests
├── test-world-generation.http       # World generation endpoint tests
├── test-world-enhancement.http      # World enhancement endpoint tests
├── test-world-storage.http          # World storage endpoint tests
└── test-world-wiki.http             # World wiki endpoint tests
```

## 🧪 Test Files Overview

### 1. **PowerShell Test Suite** - `test-api-suite.ps1`
**Most Comprehensive Testing Solution**
- ✅ **Complete API Coverage**: Tests all 15+ endpoints
- ✅ **Detailed Validation**: Structure validation and content verification  
- ✅ **Error Handling**: Tests edge cases and invalid requests
- ✅ **Visual Feedback**: Color-coded results and progress indicators
- ✅ **Cleanup Options**: Optional test data cleanup
- ✅ **Success Metrics**: Detailed pass/fail statistics

### 2. **HTTP Collection** - `test-api-collection.http`
**Manual Testing with REST Client**
- ✅ **VS Code Compatible**: Use with REST Client extension
- ✅ **Organized Tests**: Grouped by functionality (10 categories)
- ✅ **Ready-to-Run**: Pre-configured requests with sample data
- ✅ **Edge Cases**: Includes error scenarios and invalid inputs

### 3. **Quick Test Scripts**
**Fast Verification**
- `test-quick.bat` - Windows batch script
- `test-quick.sh` - Unix/Linux/macOS shell script
- ✅ **Server Health Check**: Verifies API is running
- ✅ **Basic Functionality**: Core world generation tests
- ✅ **Cross-Platform**: Works on Windows, Linux, macOS

## 🚀 How to Run Tests

### Prerequisites
1. **Start the API Server**:
   ```bash
   cd ../mdl.world
   dotnet run
   ```
   
2. **Port Configuration**: 
   - Tests automatically read the port from `../mdl.world/Properties/launchSettings.json`
   - Default fallback is `http://localhost:5000` if file cannot be read
   - The current configuration uses port `6000` as specified in launchSettings.json

3. **Verify Server**: Check that the API is responding at the correct port

### Option 1: PowerShell Test Suite (Recommended)
```powershell
# Run from the test directory
cd test

# Run complete test suite (auto-detects port from launchSettings.json)
.\test-api-suite.ps1

# Run with verbose output
.\test-api-suite.ps1 -Verbose

# Override with custom base URL
.\test-api-suite.ps1 -BaseUrl "http://localhost:8080"

# Run against different URL
.\test-api-suite.ps1 -BaseUrl "http://localhost:8080"
```

**Expected Output**:
```
🌟 Fantasy World Generator API Test Suite
Testing API at: http://localhost:5000
==================================================

🔧 INFRASTRUCTURE TESTS
[1] Testing: Get Available Themes
   ✅ PASS - Status: 200

🌍 WORLD GENERATION TESTS
[4] Testing: Basic World Generation
   ✅ PASS - Status: 200
   🆔 Generated ID: e0498e2f-3343-42e8-82fb-37d7c2c4da90

🏁 TEST RESULTS SUMMARY
Total Tests: 15
Passed: 15
Failed: 0
Success Rate: 100%

🎉 ALL TESTS PASSED! The Fantasy World Generator API is working perfectly!
```

### Option 2: HTTP Collection with VS Code
1. Install **REST Client** extension in VS Code
2. Open `test/test-api-collection.http`
3. Click "Send Request" above any test
4. View responses in VS Code

### Option 3: Quick Scripts
```bash
# Windows (run from test directory)
cd test
test-quick.bat

# Unix/Linux/macOS (make executable first)
cd test
chmod +x test-quick.sh
./test-quick.sh
```

## 📊 Test Categories

### 🔧 Infrastructure Tests
- Server health check
- Available themes retrieval
- World templates listing
- Existing worlds enumeration

### 🌍 World Generation Tests
- **Basic Generation**: Simple world creation with theme/levels
- **Custom Generation**: Advanced parameters and preferences
- **Multiple Themes**: Tests all 8 available themes
- **Parameter Validation**: Tech/magic level boundaries

### 🏰 World Management Tests
- **World Retrieval**: Get specific worlds by ID
- **World Enhancement**: Add characters, places, events, technology, magic
- **Content Validation**: Verify generated world structure

### 🤖 LLM Integration Tests
- **Text Generation**: Convert JSON world data to narratives
- **World Narratives**: Generate immersive world descriptions
- **Enhancement Service**: AI-powered world improvements

### 🧪 Edge Case Tests
- Invalid world IDs (404 errors)
- Empty requests (400 errors)
- Invalid themes and parameters
- Missing required fields

## 🎯 Test Data Examples

### Generated World Features Tested:
- **🏰 Places**: 20-25 locations (StellarSpire, NeoRealm, ArcaneHaven)
- **👥 Characters**: 10-15 figures (Digital Necromancers, Cyber-Paladins)
- **📚 Events**: 8-12 historical events (Bio-Magical Synthesis Incident)
- **⚔️ Equipment**: 10-15 items (Quantum Axes, Mana Rifles, Neural Crowns)
- **🔮 Magic**: Spell books, runes, alchemy recipes
- **🛠️ Technology**: Quantum processors, cyber interfaces, nano systems

### Validated Themes:
- ✅ Fantasy-SciFi
- ✅ Cyberpunk-Fantasy  
- ✅ Space-Magic
- ✅ Bio-Magical
- ✅ Quantum-Mystical
- ✅ Steampunk-Arcane
- ✅ Digital-Shamanism
- ✅ Techno-Druidism

## 🐛 Troubleshooting

### Common Issues:

1. **"Server appears to be down"**
   ```bash
   # Start the API server
   cd mdl.world
   dotnet run
   ```

2. **PowerShell Execution Policy Error**
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

3. **curl not found (Windows)**
   - Install curl or use PowerShell's `Invoke-RestMethod`
   - Or run the PowerShell test suite instead

4. **JSON parsing errors**
   - Check that requests have proper Content-Type headers
   - Verify JSON syntax in test files

### Performance Notes:
- **Basic tests**: ~30 seconds
- **Full test suite**: ~2-3 minutes  
- **Large world generation**: Up to 30 seconds per world

## 📈 Success Criteria

**✅ Passing Tests Should Show**:
- All infrastructure endpoints responding (200 status)
- World generation creating valid structures
- Content enhancement working
- Proper error handling (404, 400 status codes)
- Generated worlds containing expected data types

**❌ Failing Tests May Indicate**:
- Server not running or misconfigured
- Database/storage issues
- LLM service integration problems
- Network connectivity issues

## 🔄 Continuous Testing

For automated testing in CI/CD:
```bash
# Non-interactive mode
./test-api-suite.ps1 -Verbose > test-results.log
```

The test suite is designed to be idempotent and can be run repeatedly without affecting the API state (optional cleanup included).

---

**📝 Last Updated**: July 29, 2025  
**🔧 Compatible With**: .NET 8, PowerShell 5.1+, curl, VS Code REST Client

## ⚙️ Dynamic Port Configuration

### Automatic Port Detection
All test scripts now automatically read the API port from the `launchSettings.json` configuration file, ensuring tests work regardless of port changes.

**How it works:**
1. Scripts parse `../mdl.world/Properties/launchSettings.json`
2. Extract port from `profiles.http.applicationUrl`
3. Build base URL dynamically
4. Fallback to port 5000 if parsing fails

**Benefits:**
- ✅ **No Manual Updates**: Port changes automatically apply to all tests
- ✅ **Environment Consistency**: Same port used for development and testing
- ✅ **Error Prevention**: Eliminates port mismatch issues
- ✅ **Cross-Platform**: Works on Windows, Linux, and macOS

**Current Configuration:**
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:6000"
    }
  }
}
```

**Supported Test Scripts:**
- ✅ `test-api-suite.ps1` - PowerShell suite
- ✅ `test-simple.ps1` - Simple PowerShell tests  
- ✅ `test-quick.bat` - Windows batch script
- ✅ `test-quick.sh` - Unix/Linux shell script
- ⚠️ `test-api-collection.http` - Manual update required
