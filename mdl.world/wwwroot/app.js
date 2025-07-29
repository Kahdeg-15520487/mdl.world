// Enhanced functionality for the World Generator frontend

console.log('🚀 app.js loading...');

// Use globally available API_BASE or create our own if not available
const API_BASE = window.API_BASE || (() => {
    // API Configuration fallback
    function getApiBaseUrl() {
        // Priority order:
        // 1. URL parameter (e.g., ?api=http://localhost:5001)
        // 2. Environment variable (if available through meta tag)
        // 3. Local storage setting
        // 4. Current host detection
        // 5. Default fallback
        
        const urlParams = new URLSearchParams(window.location.search);
        const apiParam = urlParams.get('api');
        if (apiParam) {
            localStorage.setItem('apiBaseUrl', apiParam);
            return apiParam;
        }
        
        // Check for environment variable through meta tag
        const apiMeta = document.querySelector('meta[name="api-base-url"]');
        if (apiMeta && apiMeta.content) {
            return apiMeta.content;
        }
        
        // Check local storage
        const storedApi = localStorage.getItem('apiBaseUrl');
        if (storedApi) {
            return storedApi;
        }
        
        // Auto-detect based on current location
        if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
            // If we're not on localhost, try to use the same host with different port
            const protocol = window.location.protocol;
            const hostname = window.location.hostname;
            return `${protocol}//${hostname}:5000`;
        }
        
        // Default fallback - use current host and port for API calls
        return `${window.location.protocol}//${window.location.host}`;
    }
    
    return getApiBaseUrl();
})();

// Theme Toggle Functionality
function toggleTheme() {
    const html = document.documentElement;
    const themeToggle = document.getElementById('themeToggle');
    const currentTheme = html.getAttribute('data-theme');
    
    if (currentTheme === 'dark') {
        html.removeAttribute('data-theme');
        themeToggle.innerHTML = '<span class="theme-toggle-icon">🌙</span><span>Dark Mode</span>';
        localStorage.setItem('theme', 'light');
    } else {
        html.setAttribute('data-theme', 'dark');
        themeToggle.innerHTML = '<span class="theme-toggle-icon">☀️</span><span>Light Mode</span>';
        localStorage.setItem('theme', 'dark');
    }
}

// Initialize theme on page load
function initializeTheme() {
    const savedTheme = localStorage.getItem('theme');
    const html = document.documentElement;
    const themeToggle = document.getElementById('themeToggle');
    
    if (savedTheme === 'dark') {
        html.setAttribute('data-theme', 'dark');
        if (themeToggle) {
            themeToggle.innerHTML = '<span class="theme-toggle-icon">☀️</span><span>Light Mode</span>';
        }
    }
}

// Configuration UI functions
function showApiConfig() {
    const currentApi = API_BASE;
    const newApi = prompt(`Current API Base URL: ${currentApi}\n\nEnter new API Base URL:`, currentApi);
    if (newApi && newApi !== currentApi) {
        localStorage.setItem('apiBaseUrl', newApi);
        alert('API URL updated! Please refresh the page to apply changes.');
    }
}

function resetApiConfig() {
    localStorage.removeItem('apiBaseUrl');
    alert('API configuration reset! Please refresh the page to apply changes.');
}

function setApiUrl(url) {
    localStorage.setItem('apiBaseUrl', url);
    alert(`API URL set to: ${url}\nPlease refresh the page to apply changes.`);
}

async function testApiConnection() {
    const result = document.getElementById('configResult');
    result.innerHTML = '<div class="loading">Testing API connection...</div>';
    
    try {
        const response = await fetch(API_BASE + '/World', {
            method: 'GET',
            timeout: 5000
        });
        
        if (response.ok) {
            result.innerHTML = `
                <div class="success">
                    <h3>✅ Connection Successful</h3>
                    <p>Successfully connected to API at: <strong>${API_BASE}</strong></p>
                    <p>Response status: ${response.status} ${response.statusText}</p>
                </div>
            `;
        } else {
            result.innerHTML = `
                <div class="error">
                    <h3>⚠️ Connection Issues</h3>
                    <p>API responded but with error status: ${response.status} ${response.statusText}</p>
                    <p>URL: ${API_BASE}</p>
                </div>
            `;
        }
    } catch (error) {
        result.innerHTML = `
            <div class="error">
                <h3>❌ Connection Failed</h3>
                <p>Could not connect to API at: <strong>${API_BASE}</strong></p>
                <p>Error: ${error.message}</p>
                <p>Make sure the API server is running and accessible.</p>
            </div>
        `;
    }
}

function updateConfigDisplay() {
    const apiUrlElement = document.getElementById('currentApiUrl');
    if (apiUrlElement) {
        apiUrlElement.textContent = API_BASE;
    }
}

// LLM Configuration Functions
async function loadLlmConfig() {
    try {
        const response = await fetch(API_BASE + '/api/Configuration/llm');
        if (response.ok) {
            const config = await response.json();
            
            const llmUrlElement = document.getElementById('currentLlmUrl');
            const llmModelElement = document.getElementById('currentLlmModel');
            const llmUrlInput = document.getElementById('llmBaseUrl');
            const llmModelInput = document.getElementById('llmModel');
            
            if (llmUrlElement) llmUrlElement.textContent = config.baseUrl;
            if (llmModelElement) llmModelElement.textContent = config.model;
            if (llmUrlInput) llmUrlInput.value = config.baseUrl;
            if (llmModelInput) llmModelInput.value = config.model;
            
            console.log('LLM configuration loaded:', config);
        } else {
            console.error('Failed to load LLM configuration');
        }
    } catch (error) {
        console.error('Error loading LLM configuration:', error);
    }
}

async function updateLlmConfig() {
    const baseUrl = document.getElementById('llmBaseUrl').value;
    const model = document.getElementById('llmModel').value;
    
    if (!baseUrl) {
        alert('Please enter a LLM base URL');
        return;
    }
    
    try {
        const response = await fetch(API_BASE + '/api/Configuration/llm', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                baseUrl: baseUrl,
                model: model || null
            })
        });
        
        if (response.ok) {
            const result = await response.json();
            alert('✅ LLM configuration updated successfully!');
            await loadLlmConfig(); // Reload to show updated values
            console.log('LLM configuration updated:', result);
        } else {
            const error = await response.text();
            alert('❌ Failed to update LLM configuration: ' + error);
        }
    } catch (error) {
        alert('❌ Error updating LLM configuration: ' + error.message);
    }
}

async function testLlmConnection() {
    const result = document.getElementById('configResult');
    result.innerHTML = '<div class="loading">Testing LLM connection...</div>';
    
    try {
        const response = await fetch(API_BASE + '/api/Configuration/llm/test');
        
        if (response.ok) {
            const health = await response.json();
            
            if (health.isAvailable) {
                result.innerHTML = `
                    <div class="success">
                        <h3>✅ LLM Connection Successful</h3>
                        <p><strong>Status:</strong> ${health.status}</p>
                        <p><strong>Base URL:</strong> ${health.baseUrl}</p>
                        <p><strong>Model:</strong> ${health.model}</p>
                        <p><strong>Response Time:</strong> ${health.responseTimeMs}ms</p>
                        <p><strong>Checked At:</strong> ${new Date(health.checkedAt).toLocaleString()}</p>
                    </div>
                `;
            } else {
                result.innerHTML = `
                    <div class="error">
                        <h3>⚠️ LLM Connection Issues</h3>
                        <p><strong>Status:</strong> ${health.status}</p>
                        <p><strong>Base URL:</strong> ${health.baseUrl}</p>
                        <p><strong>Model:</strong> ${health.model}</p>
                        ${health.errorMessage ? `<p><strong>Error:</strong> ${health.errorMessage}</p>` : ''}
                        <p>Make sure the LLM server is running and accessible.</p>
                    </div>
                `;
            }
        } else {
            result.innerHTML = `
                <div class="error">
                    <h3>❌ LLM Test Failed</h3>
                    <p>Could not test LLM connection</p>
                    <p>Response status: ${response.status} ${response.statusText}</p>
                </div>
            `;
        }
    } catch (error) {
        result.innerHTML = `
            <div class="error">
                <h3>❌ LLM Connection Failed</h3>
                <p>Could not connect to LLM service</p>
                <p>Error: ${error.message}</p>
                <p>Make sure the API server is running and LLM service is configured.</p>
            </div>
        `;
    }
}

function setLlmConfig(baseUrl, model) {
    document.getElementById('llmBaseUrl').value = baseUrl;
    document.getElementById('llmModel').value = model;
    alert(`LLM configuration preset applied!\nBase URL: ${baseUrl}\nModel: ${model}\n\nClick "Update LLM Config" to save changes.`);
}

// Form Utilities
function updateRangeValue(rangeId) {
    const range = document.getElementById(rangeId);
    const valueSpan = document.getElementById(rangeId + 'Value');
    if (range && valueSpan) {
        valueSpan.textContent = range.value;
    }
}

function toggleAdvancedOptions() {
    const generationType = document.getElementById('generationType').value;
    const advancedOptions = document.getElementById('advancedOptions');
    
    if (generationType === 'complete') {
        advancedOptions.classList.remove('hidden');
    } else {
        advancedOptions.classList.add('hidden');
    }
}

// Search functionality
function handleSearch() {
    const searchTerm = document.getElementById('worldSearch').value;
    const filterType = document.getElementById('worldFilter').value;
    
    const filteredWorlds = currentWorlds.filter(world => {
        const matchesSearch = !searchTerm || 
            world.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            (world.description && world.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
            world.theme.toLowerCase().includes(searchTerm.toLowerCase());
            
        const matchesFilter = filterType === 'all' || 
            world.genre.toLowerCase().includes(filterType.toLowerCase()) ||
            world.theme.toLowerCase().includes(filterType.toLowerCase());
            
        return matchesSearch && matchesFilter;
    });
    
    displayWorlds(filteredWorlds);
}

// Initialize form submission handler
function initializeFormHandler() {
    const generateForm = document.getElementById('generateForm');
    if (generateForm) {
        generateForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const btn = document.getElementById('generateBtn');
            const result = document.getElementById('generateResult');
            
            btn.disabled = true;
            btn.textContent = '🔄 Generating...';
            result.innerHTML = '<div class="loading">Creating your world...</div>';
            
            try {
                const formData = new FormData(e.target);
                const generationType = formData.get('generationType');
                
                let endpoint, payload;
                
                if (generationType === 'basic') {
                    endpoint = '/World/generate';
                    payload = {
                        worldName: formData.get('worldName'),
                        theme: formData.get('theme'),
                        techLevel: parseInt(formData.get('techLevel')),
                        magicLevel: parseInt(formData.get('magicLevel'))
                    };
                } else {
                    endpoint = '/World/generate-complete';
                    payload = {
                        worldName: formData.get('worldName'),
                        theme: formData.get('theme'),
                        techLevel: parseInt(formData.get('techLevel')),
                        magicLevel: parseInt(formData.get('magicLevel')),
                        totalPlaces: parseInt(formData.get('totalPlaces')),
                        characterCount: parseInt(formData.get('characterCount')),
                        equipmentCount: parseInt(formData.get('equipmentCount')),
                        difficultyLevel: formData.get('difficultyLevel'),
                        includeMagicTech: true,
                        includeAncientRuins: true
                    };
                }
                
                const response = await fetch(API_BASE + endpoint, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(payload)
                });
                
                if (!response.ok) {
                    throw new Error('Failed to generate world');
                }
                
                const world = await response.json();
                
                result.innerHTML = `
                    <div class="success">
                        <h3>✅ World Created Successfully!</h3>
                        <p><strong>Name:</strong> ${world.name}</p>
                        <p><strong>ID:</strong> ${world.id}</p>
                        <p><strong>Places:</strong> ${world.places.length}</p>
                        <p><strong>Characters:</strong> ${world.historicFigures.length}</p>
                        <p><strong>Items:</strong> ${world.equipment.length}</p>
                        <div style="margin-top: 15px;">
                            <button class="btn" onclick="viewWorldWiki('${world.id}')">📖 View Wiki</button>
                            <button class="btn btn-secondary" onclick="showTab('manage')">📂 Go to My Worlds</button>
                        </div>
                    </div>
                `;
                
                // Reset form
                e.target.reset();
                updateRangeValue('techLevel');
                updateRangeValue('magicLevel');
                
            } catch (error) {
                result.innerHTML = `
                    <div class="error">
                        <h3>❌ Error</h3>
                        <p>Failed to generate world: ${error.message}</p>
                    </div>
                `;
            } finally {
                btn.disabled = false;
                btn.textContent = '🌟 Generate World';
            }
        });
    }
}

// Global variables
let currentWorlds = [];

// LLM Service Status Manager
class LLMServiceStatus {
    static isAvailable = false;
    static lastCheck = null;
    static checkInterval = null;

    static async checkAvailability() {
        try {
            const response = await fetch(`${API_BASE}/api/TextGeneration/available`);
            const data = await response.json();
            this.isAvailable = data.isAvailable;
            this.lastCheck = new Date();
            this.updateUI();
            return this.isAvailable;
        } catch (error) {
            console.error('Error checking LLM service availability:', error);
            this.isAvailable = false;
            this.updateUI();
            return false;
        }
    }

    static async getDetailedHealth() {
        try {
            const response = await fetch(`${API_BASE}/api/TextGeneration/health`);
            const health = await response.json();
            this.isAvailable = health.isAvailable;
            this.lastCheck = new Date();
            this.updateUI();
            return health;
        } catch (error) {
            console.error('Error getting LLM service health:', error);
            this.isAvailable = false;
            this.updateUI();
            return null;
        }
    }

    static updateUI() {
        const statusIndicator = document.getElementById('llm-status');
        if (!statusIndicator) return;

        statusIndicator.className = `status-indicator ${this.isAvailable ? 'available' : 'unavailable'}`;
        statusIndicator.title = this.isAvailable 
            ? 'LLM Service Available' 
            : 'LLM Service Unavailable';
        
        // Update any enhancement buttons
        const enhanceButtons = document.querySelectorAll('.enhance-btn');
        enhanceButtons.forEach(btn => {
            btn.disabled = !this.isAvailable;
            if (!this.isAvailable) {
                btn.title = 'LLM Service unavailable';
            }
        });
    }

    static startPeriodicCheck(intervalMs = 30000) { // Check every 30 seconds
        this.stopPeriodicCheck();
        this.checkAvailability(); // Initial check
        this.checkInterval = setInterval(() => this.checkAvailability(), intervalMs);
    }

    static stopPeriodicCheck() {
        if (this.checkInterval) {
            clearInterval(this.checkInterval);
            this.checkInterval = null;
        }
    }

    static async showHealthModal() {
        const health = await this.getDetailedHealth();
        if (!health) {
            NotificationManager.error('Could not retrieve service health information');
            return;
        }

        const modal = new Modal('LLM Service Health', `
            <div class="health-info">
                <div class="health-status ${health.isAvailable ? 'healthy' : 'unhealthy'}">
                    <h4>Status: ${health.status}</h4>
                </div>
                <div class="health-details">
                    <p><strong>Base URL:</strong> ${health.baseUrl}</p>
                    <p><strong>Model:</strong> ${health.model}</p>
                    <p><strong>Response Time:</strong> ${health.responseTimeMs}ms</p>
                    <p><strong>Last Checked:</strong> ${new Date(health.checkedAt).toLocaleString()}</p>
                    ${health.errorMessage ? `<p><strong>Error:</strong> ${health.errorMessage}</p>` : ''}
                </div>
            </div>
        `);
        modal.show();
    }
}

class NotificationManager {
    static show(message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        
        document.body.appendChild(notification);
        
        // Trigger animation
        setTimeout(() => notification.classList.add('show'), 100);
        
        // Auto remove
        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => document.body.removeChild(notification), 300);
        }, duration);
    }
    
    static success(message) {
        this.show(message, 'success');
    }
    
    static error(message) {
        this.show(message, 'error');
    }
    
    static info(message) {
        this.show(message, 'info');
    }
}

class ProgressManager {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        this.progressBar = null;
    }
    
    show(message = 'Processing...') {
        this.container.innerHTML = `
            <div class="loading">
                <p>${message}</p>
                <div class="progress-bar">
                    <div class="progress-fill"></div>
                </div>
            </div>
        `;
        this.progressBar = this.container.querySelector('.progress-fill');
    }
    
    update(percentage) {
        if (this.progressBar) {
            this.progressBar.style.width = percentage + '%';
        }
    }
    
    hide() {
        this.container.innerHTML = '';
    }
}

class Modal {
    constructor(title, content) {
        this.modal = document.createElement('div');
        this.modal.className = 'modal';
        this.modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3>${title}</h3>
                    <button class="modal-close">&times;</button>
                </div>
                <div class="modal-body">
                    ${content}
                </div>
            </div>
        `;
        
        // Close handlers
        this.modal.querySelector('.modal-close').addEventListener('click', () => this.close());
        this.modal.addEventListener('click', (e) => {
            if (e.target === this.modal) this.close();
        });
        
        document.body.appendChild(this.modal);
    }
    
    show() {
        setTimeout(() => this.modal.classList.add('show'), 100);
        return this;
    }
    
    close() {
        this.modal.classList.remove('show');
        setTimeout(() => document.body.removeChild(this.modal), 300);
    }
}

// Enhanced World Management Functions
async function showWorldDetails(worldId) {
    try {
        const response = await fetch(`${API_BASE}/World/${worldId}`);
        if (!response.ok) throw new Error('Failed to load world details');
        
        const world = await response.json();
        
        const modal = new Modal('World Details', `
            <div class="card-grid">
                <div class="info-card">
                    <h4>Basic Info</h4>
                    <p><strong>Name:</strong> ${world.name}</p>
                    <p><strong>Theme:</strong> ${world.worldInfo.activeThemes.join(', ') || 'N/A'}</p>
                    <p><strong>Genre:</strong> ${world.worldInfo.genre}</p>
                    <p><strong>Created:</strong> ${new Date(world.creationDate).toLocaleDateString()}</p>
                </div>
                
                <div class="info-card">
                    <h4>Content</h4>
                    <p><strong>Places:</strong> ${world.places.length}</p>
                    <p><strong>Characters:</strong> ${world.historicFigures.length}</p>
                    <p><strong>Events:</strong> ${world.worldEvents.length}</p>
                    <p><strong>Equipment:</strong> ${world.equipment.length}</p>
                </div>
                
                <div class="info-card">
                    <h4>Magic & Technology</h4>
                    <p><strong>Spell Books:</strong> ${world.spellBooks.length}</p>
                    <p><strong>Runes:</strong> ${world.runesOfPower.length}</p>
                    <p><strong>Alchemy:</strong> ${world.alchemyRecipes.length}</p>
                    <p><strong>Tech Specs:</strong> ${world.technicalSpecs.length}</p>
                </div>
                
                <div class="info-card">
                    <h4>World Laws</h4>
                    <p><strong>Magic Exists:</strong> ${world.worldInfo.laws.magicExists ? 'Yes' : 'No'}</p>
                    <p><strong>Death Permanent:</strong> ${world.worldInfo.laws.deathIsPermanent ? 'Yes' : 'No'}</p>
                    <p><strong>Time Travel:</strong> ${world.worldInfo.laws.timeTravel ? 'Yes' : 'No'}</p>
                    <p><strong>Multiverse:</strong> ${world.worldInfo.laws.multiverse ? 'Yes' : 'No'}</p>
                </div>
            </div>
            
            <div style="margin-top: 20px;">
                <button class="btn" onclick="viewWorldWiki('${world.id}'); closeModal()">📖 View Wiki</button>
                <button class="btn btn-secondary" onclick="enhanceWorld('${world.id}'); closeModal()">✨ Enhance</button>
            </div>
        `);
        
        modal.show();
        
        // Store reference for closing
        window.currentModal = modal;
        
    } catch (error) {
        NotificationManager.error('Failed to load world details: ' + error.message);
    }
}

// Wiki Functions
async function loadWikiWorldOptions() {
    const select = document.getElementById('wikiWorldSelect');
    
    if (!select) {
        console.error('wikiWorldSelect element not found');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/World`);
        if (!response.ok) {
            throw new Error('Failed to load worlds');
        }
        
        const worlds = await response.json();
        
        select.innerHTML = '<option value="">Select a world...</option>' +
            worlds.map(world => `<option value="${world.id}">${world.name}</option>`).join('');
        
        console.log('Loaded', worlds.length, 'worlds into select');
        
    } catch (error) {
        console.error('Error loading world options:', error);
        select.innerHTML = '<option value="">Error loading worlds</option>';
        NotificationManager.error('Failed to load world options: ' + error.message);
    }
}

function loadWikiOptions() {
    const wikiType = document.getElementById('wikiType');
    const specificIdGroup = document.getElementById('specificIdGroup');
    
    if (!wikiType || !specificIdGroup) {
        console.error('Wiki elements not found');
        return;
    }
    
    if (wikiType.value === 'world') {
        specificIdGroup.style.display = 'none';
    } else {
        specificIdGroup.style.display = 'block';
        const specificId = document.getElementById('specificId');
        if (specificId) {
            specificId.placeholder = `Enter ${wikiType.value} ID`;
        }
    }
}

function loadWiki() {
    const worldIdSelect = document.getElementById('wikiWorldSelect');
    const wikiType = document.getElementById('wikiType');
    const specificId = document.getElementById('specificId');
    
    if (!worldIdSelect || !wikiType) {
        console.error('Required wiki elements not found');
        NotificationManager.error('Wiki interface not properly loaded');
        return;
    }
    
    const worldId = worldIdSelect.value;
    
    console.log('Loading wiki for world:', worldId);
    
    if (!worldId) {
        NotificationManager.error('Please select a world first');
        return;
    }
    
    let url = `${API_BASE}/World/${worldId}/wiki`;
    
    if (wikiType.value !== 'world') {
        if (!specificId || !specificId.value) {
            NotificationManager.error(`Please enter a ${wikiType.value} ID`);
            return;
        }
        url += `/${wikiType.value}/${specificId.value}`;
    }
    
    console.log('Loading wiki URL:', url);
    
    const frame = document.getElementById('wikiFrame');
    if (!frame) {
        console.error('wikiFrame element not found');
        NotificationManager.error('Wiki frame not found');
        return;
    }
    
    frame.src = url;
    frame.classList.remove('hidden');
    
    NotificationManager.success('Loading wiki...');
}

function closeModal() {
    if (window.currentModal) {
        window.currentModal.close();
        window.currentModal = null;
    }
}

function viewWorldWiki(worldId) {
    console.log('viewWorldWiki called with worldId:', worldId);
    
    // Simple and reliable: use URL hash parameter to navigate to wiki tab with world pre-selected
    window.location.hash = `wiki-${worldId}`;
    
    // Also switch to the wiki tab
    showTab('wiki');
    
    NotificationManager.info('Loading wiki...');
}

// Local Storage for user preferences
class UserPreferences {
    static save(key, value) {
        localStorage.setItem(`worldgen_${key}`, JSON.stringify(value));
    }
    
    static load(key, defaultValue = null) {
        const stored = localStorage.getItem(`worldgen_${key}`);
        return stored ? JSON.parse(stored) : defaultValue;
    }
    
    static saveFormData() {
        const form = document.getElementById('generateForm');
        if (!form) return;
        
        const formData = new FormData(form);
        const data = {};
        
        for (let [key, value] of formData.entries()) {
            data[key] = value;
        }
        
        this.save('lastFormData', data);
    }
    
    static loadFormData() {
        const data = this.load('lastFormData');
        if (!data) return;
        
        const form = document.getElementById('generateForm');
        if (!form) return;
        
        Object.entries(data).forEach(([key, value]) => {
            const field = form.elements[key];
            if (field) {
                field.value = value;
                
                // Update range displays
                if (field.type === 'range' && window.updateRangeValue) {
                    window.updateRangeValue(key);
                }
            }
        });
        
        // Update advanced options visibility
        if (window.toggleAdvancedOptions) {
            window.toggleAdvancedOptions();
        }
    }
}

// Search and filter functionality for worlds
function filterWorlds(searchTerm, filterType = 'all') {
    const filteredWorlds = currentWorlds.filter(world => {
        const matchesSearch = !searchTerm || 
            world.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            (world.description && world.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
            (world.theme && world.theme.toLowerCase().includes(searchTerm.toLowerCase()));
            
        const matchesFilter = filterType === 'all' || 
            (world.genre && world.genre.toLowerCase() === filterType.toLowerCase()) ||
            (world.theme && world.theme.toLowerCase().includes(filterType.toLowerCase()));
            
        return matchesSearch && matchesFilter;
    });
    
    displayWorlds(filteredWorlds);
}

// Hash-based navigation handler
function handleHashNavigation() {
    const hash = window.location.hash;
    console.log('Hash changed to:', hash);
    
    if (hash.startsWith('#wiki-')) {
        const worldId = hash.substring(6); // Remove '#wiki-' prefix
        console.log('Navigating to wiki for world:', worldId);
        
        // Switch to wiki tab
        showTab('wiki');
        
        // Wait for tab to load, then set up the wiki
        setTimeout(async () => {
            try {
                // Load world options
                console.log('Loading world options...');
                await loadWikiWorldOptions();
                
                // Wait longer for select to be populated
                await new Promise(resolve => setTimeout(resolve, 500));
                
                // Set the world selection
                const worldSelect = document.getElementById('wikiWorldSelect');
                const wikiType = document.getElementById('wikiType');
                
                if (worldSelect && wikiType) {
                    console.log('Available world options:', Array.from(worldSelect.options).map(o => `${o.value}: ${o.text}`));
                    
                    worldSelect.value = worldId;
                    wikiType.value = 'world';
                    
                    console.log('Set world select to:', worldSelect.value);
                    console.log('World found in options:', worldSelect.selectedIndex > 0);
                    
                    // Verify the value was actually set
                    if (worldSelect.value === worldId) {
                        // Load wiki options and then load the wiki
                        loadWikiOptions();
                        setTimeout(() => loadWiki(), 200);
                    } else {
                        console.error('World ID not found in dropdown options');
                        NotificationManager.error('World not found in dropdown. Opening wiki directly...');
                        const wikiUrl = `${API_BASE}/World/${worldId}/wiki`;
                        window.open(wikiUrl, '_blank');
                    }
                } else {
                    console.error('Wiki elements not found');
                    // Fallback: open wiki URL directly
                    const wikiUrl = `${API_BASE}/World/${worldId}/wiki`;
                    window.open(wikiUrl, '_blank');
                }
            } catch (error) {
                console.error('Error setting up wiki from hash:', error);
                // Fallback: open wiki URL directly
                const wikiUrl = `${API_BASE}/World/${worldId}/wiki`;
                window.open(wikiUrl, '_blank');
            }
        }, 800); // Increased timeout to give more time for everything to load
    }
}

// Keyboard shortcuts
document.addEventListener('keydown', (e) => {
    // Ctrl/Cmd + K to focus search
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        const searchInput = document.getElementById('worldSearch');
        if (searchInput) searchInput.focus();
    }
    
    // Escape to close modals
    if (e.key === 'Escape' && window.currentModal) {
        closeModal();
    }
});

// Enhanced generation with progress
async function generateWorldWithProgress(formData, generationType) {
    const progress = new ProgressManager('generateResult');
    
    try {
        progress.show('Initializing world generation...');
        progress.update(10);
        
        let endpoint, payload;
        
        if (generationType === 'basic') {
            endpoint = '/World/generate';
            payload = {
                worldName: formData.get('worldName'),
                theme: formData.get('theme'),
                techLevel: parseInt(formData.get('techLevel')),
                magicLevel: parseInt(formData.get('magicLevel'))
            };
        } else {
            endpoint = '/World/generate-complete';
            payload = {
                worldName: formData.get('worldName'),
                theme: formData.get('theme'),
                techLevel: parseInt(formData.get('techLevel')),
                magicLevel: parseInt(formData.get('magicLevel')),
                totalPlaces: parseInt(formData.get('totalPlaces')),
                characterCount: parseInt(formData.get('characterCount')),
                equipmentCount: parseInt(formData.get('equipmentCount')),
                difficultyLevel: formData.get('difficultyLevel'),
                includeMagicTech: true,
                includeAncientRuins: true
            };
        }
        
        progress.update(30);
        
        const response = await fetch(API_BASE + endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });
        
        progress.update(70);
        
        if (!response.ok) {
            throw new Error('Failed to generate world');
        }
        
        const world = await response.json();
        progress.update(100);
        
        setTimeout(() => {
            progress.hide();
            NotificationManager.success(`World "${world.name}" created successfully!`);
            
            const resultDiv = document.getElementById('generateResult');
            if (resultDiv) {
                resultDiv.innerHTML = `
                    <div class="success">
                        <h3>✅ World Created Successfully!</h3>
                        <p><strong>Name:</strong> ${world.name}</p>
                        <p><strong>ID:</strong> ${world.id}</p>
                        <p><strong>Places:</strong> ${world.places.length}</p>
                        <p><strong>Characters:</strong> ${world.historicFigures.length}</p>
                        <p><strong>Items:</strong> ${world.equipment.length}</p>
                        <div style="margin-top: 15px;">
                            <button class="btn" onclick="viewWorldWiki('${world.id}')">📖 View Wiki</button>
                            <button class="btn btn-secondary" onclick="showWorldDetails('${world.id}')">ℹ️ Details</button>
                            <button class="btn btn-secondary" onclick="showTab('manage')">📂 Go to My Worlds</button>
                        </div>
                    </div>
                `;
            }
        }, 500);
        
        return world;
        
    } catch (error) {
        progress.hide();
        NotificationManager.error('Failed to generate world: ' + error.message);
        throw error;
    }
}

// World Management Functions
async function loadWorlds() {
    const worldsList = document.getElementById('worldsList');
    worldsList.innerHTML = '<div class="loading">Loading worlds...</div>';
    
    try {
        const response = await fetch(`${API_BASE}/World`);
        if (!response.ok) {
            throw new Error('Failed to load worlds');
        }
        
        currentWorlds = await response.json();
        displayWorlds(currentWorlds);
        
    } catch (error) {
        worldsList.innerHTML = `
            <div class="error">
                <h3>❌ Error</h3>
                <p>Failed to load worlds: ${error.message}</p>
                <button class="btn" onclick="loadWorlds()">🔄 Retry</button>
            </div>
        `;
        NotificationManager.error('Failed to load worlds: ' + error.message);
    }
}

function displayWorlds(worlds) {
    const worldsList = document.getElementById('worldsList');
    
    if (worlds.length === 0) {
        worldsList.innerHTML = `
            <div style="text-align: center; padding: 40px; color: #6c757d;">
                <h3>No worlds found</h3>
                <p>Try adjusting your search or filters</p>
            </div>
        `;
        return;
    }
    
    const worldsHtml = worlds.map(world => `
        <div class="world-card" onclick="showWorldDetails('${world.id}')">
            <h3>${world.name}</h3>
            <p>${world.description || 'No description available'}</p>
            <div class="world-meta">
                <span>${world.genre || 'Unknown'} • ${world.theme || 'No theme'}</span>
                <span>${formatFileSize(world.fileSizeBytes)}</span>
            </div>
            <div class="world-meta">
                <span>📍 ${world.places?.length || 0} places • 👥 ${world.historicFigures?.length || 0} characters • ⚔️ ${world.equipment?.length || 0} items</span>
            </div>
            <div class="world-actions" onclick="event.stopPropagation();">
                <button class="btn" onclick="viewWorldWiki('${world.id}')">📖 Wiki</button>
                <button class="btn btn-secondary" onclick="enhanceWorld('${world.id}')">✨ Enhance</button>
                <button class="btn btn-secondary" onclick="copyWorld('${world.id}')">📋 Copy</button>
                <button class="btn btn-secondary" onclick="exportWorld('${world.id}')">💾 Export</button>
                <button class="btn btn-danger" onclick="deleteWorld('${world.id}')">🗑️ Delete</button>
            </div>
        </div>
    `).join('');
    
    worldsList.innerHTML = `<div class="world-list">${worldsHtml}</div>`;
}

function formatFileSize(bytes) {
    if (!bytes) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

async function enhanceWorld(worldId) {
    if (!LLMServiceStatus.isAvailable) {
        NotificationManager.error('LLM Service is not available for world enhancement');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/WorldEnhancement/enhance/${worldId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        if (!response.ok) {
            throw new Error('Failed to enhance world');
        }
        
        const result = await response.json();
        NotificationManager.success('World enhanced successfully!');
        
        // Refresh worlds list
        loadWorlds();
        
    } catch (error) {
        NotificationManager.error('Failed to enhance world: ' + error.message);
    }
}

async function copyWorld(worldId) {
    try {
        const response = await fetch(`${API_BASE}/World/${worldId}/copy`, {
            method: 'POST'
        });
        
        if (!response.ok) {
            throw new Error('Failed to copy world');
        }
        
        const newWorld = await response.json();
        NotificationManager.success(`World copied as "${newWorld.name}"`);
        
        // Refresh worlds list
        loadWorlds();
        
    } catch (error) {
        NotificationManager.error('Failed to copy world: ' + error.message);
    }
}

async function exportWorld(worldId) {
    try {
        const response = await fetch(`${API_BASE}/World/${worldId}/export`);
        
        if (!response.ok) {
            throw new Error('Failed to export world');
        }
        
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `world-${worldId}.json`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        
        NotificationManager.success('World exported successfully!');
        
    } catch (error) {
        NotificationManager.error('Failed to export world: ' + error.message);
    }
}

async function deleteWorld(worldId) {
    if (!confirm('Are you sure you want to delete this world? This action cannot be undone.')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/World/${worldId}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            throw new Error('Failed to delete world');
        }
        
        NotificationManager.success('World deleted successfully!');
        
        // Refresh worlds list
        loadWorlds();
        
    } catch (error) {
        NotificationManager.error('Failed to delete world: ' + error.message);
    }
}

// Initialize enhanced features
document.addEventListener('DOMContentLoaded', () => {
    // Load user preferences
    UserPreferences.loadFormData();
    
    // Save form data on changes
    const form = document.getElementById('generateForm');
    if (form) {
        form.addEventListener('change', () => {
            UserPreferences.saveFormData();
        });
    }
    
    // Start LLM service monitoring
    LLMServiceStatus.startPeriodicCheck();
    
    // Handle initial hash navigation
    handleHashNavigation();
});

// Listen for hash changes
window.addEventListener('hashchange', handleHashNavigation);

function showTab(tabName) {
    console.log('Switching to tab:', tabName);
    
    // Hide all tab panels
    document.querySelectorAll('.tab-panel').forEach(panel => {
        panel.classList.remove('active');
    });
    
    // Remove active class from all tabs
    document.querySelectorAll('.tab').forEach(tab => {
        tab.classList.remove('active');
    });
    
    // Show selected tab panel
    const tabPanel = document.getElementById(tabName + '-tab');
    if (tabPanel) {
        tabPanel.classList.add('active');
        console.log('Activated tab panel:', tabName + '-tab');
    } else {
        console.error('Tab panel not found:', tabName + '-tab');
    }
    
    // Add active class to the corresponding tab button
    const tabButton = document.querySelector(`[onclick="showTab('${tabName}')"]`);
    if (tabButton) {
        tabButton.classList.add('active');
        console.log('Activated tab button for:', tabName);
    } else {
        console.error('Tab button not found for:', tabName);
    }
    
    // Load data for specific tabs
    if (tabName === 'manage') {
        loadWorlds();
    } else if (tabName === 'wiki') {
        console.log('Loading wiki world options...');
        loadWikiWorldOptions();
    }
}

// Export enhanced functions to global scope
window.NotificationManager = NotificationManager;
window.ProgressManager = ProgressManager;
window.Modal = Modal;
window.LLMServiceStatus = LLMServiceStatus;
window.showWorldDetails = showWorldDetails;
window.closeModal = closeModal;
window.loadWikiWorldOptions = loadWikiWorldOptions;
window.loadWikiOptions = loadWikiOptions;
window.loadWiki = loadWiki;
window.viewWorldWiki = viewWorldWiki;
window.handleHashNavigation = handleHashNavigation;
window.generateWorldWithProgress = generateWorldWithProgress;
window.loadWorlds = loadWorlds;
window.displayWorlds = displayWorlds;
window.formatFileSize = formatFileSize;
window.enhanceWorld = enhanceWorld;
window.copyWorld = copyWorld;
window.exportWorld = exportWorld;
window.deleteWorld = deleteWorld;
window.UserPreferences = UserPreferences;
window.filterWorlds = filterWorlds;
window.showTab = showTab;
window.getConfigurationSource = getConfigurationSource;

// Add new functions from HTML
window.toggleTheme = toggleTheme;
window.initializeTheme = initializeTheme;
window.showApiConfig = showApiConfig;
window.resetApiConfig = resetApiConfig;
window.setApiUrl = setApiUrl;
window.testApiConnection = testApiConnection;
window.updateConfigDisplay = updateConfigDisplay;
window.loadLlmConfig = loadLlmConfig;
window.updateLlmConfig = updateLlmConfig;
window.testLlmConnection = testLlmConnection;
window.setLlmConfig = setLlmConfig;
window.updateRangeValue = updateRangeValue;
window.toggleAdvancedOptions = toggleAdvancedOptions;
window.handleSearch = handleSearch;

// Initialize application when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    console.log('🎯 Initializing Fantasy World Generator...');
    
    // Initialize theme first
    initializeTheme();
    
    // Initialize range values
    updateRangeValue('techLevel');
    updateRangeValue('magicLevel');
    
    // Update configuration display
    updateConfigDisplay();
    
    // Load LLM configuration
    loadLlmConfig();
    
    // Set up wiki type change handler
    const wikiTypeElement = document.getElementById('wikiType');
    if (wikiTypeElement) {
        wikiTypeElement.addEventListener('change', loadWikiOptions);
    }
    
    // Initialize LLM service status monitoring
    LLMServiceStatus.startPeriodicCheck();
    
    // Handle hash navigation on page load
    handleHashNavigation();
    
    // Initialize form submission handler
    initializeFormHandler();
    
    // Log current API configuration for debugging
    console.log('Fantasy World Generator initialized');
    console.log('API Base URL:', API_BASE);
    console.log('Configuration source:', getConfigurationSource());
});

function getConfigurationSource() {
    const urlParams = new URLSearchParams(window.location.search);
    const apiParam = urlParams.get('api');
    if (apiParam) return 'URL Parameter';
    
    const apiMeta = document.querySelector('meta[name="api-base-url"]');
    if (apiMeta && apiMeta.content) return 'Meta Tag';
    
    const storedApi = localStorage.getItem('apiBaseUrl');
    if (storedApi) return 'Local Storage';
    
    if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
        return 'Auto-detected Host';
    }
    
    return 'Default Fallback';
}
