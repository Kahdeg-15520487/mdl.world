// Enhanced functionality for the World Generator frontend

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

function closeModal() {
    if (window.currentModal) {
        window.currentModal.close();
        window.currentModal = null;
    }
}

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
            
            document.getElementById('generateResult').innerHTML = `
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
        }, 500);
        
        return world;
        
    } catch (error) {
        progress.hide();
        NotificationManager.error('Failed to generate world: ' + error.message);
        throw error;
    }
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
        
        Object.entries(data).forEach(([key, value]) => {
            const field = form.elements[key];
            if (field) {
                field.value = value;
                
                // Update range displays
                if (field.type === 'range') {
                    updateRangeValue(key);
                }
            }
        });
        
        // Update advanced options visibility
        toggleAdvancedOptions();
    }
}

// Search and filter functionality for worlds
function filterWorlds(searchTerm, filterType = 'all') {
    const filteredWorlds = currentWorlds.filter(world => {
        const matchesSearch = !searchTerm || 
            world.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            world.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
            world.theme.toLowerCase().includes(searchTerm.toLowerCase());
            
        const matchesFilter = filterType === 'all' || 
            world.genre.toLowerCase() === filterType.toLowerCase() ||
            world.theme.toLowerCase().includes(filterType.toLowerCase());
            
        return matchesSearch && matchesFilter;
    });
    
    displayWorlds(filteredWorlds);
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
                <span>${world.genre} • ${world.theme}</span>
                <span>${formatFileSize(world.fileSizeBytes)}</span>
            </div>
            <div class="world-meta">
                <span>📍 ${world.placeCount} places • 👥 ${world.characterCount} characters • ⚔️ ${world.itemCount} items</span>
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

// Initialize enhanced features
document.addEventListener('DOMContentLoaded', () => {
    // Load user preferences
    UserPreferences.loadFormData();
    
    // Save form data on changes
    const form = document.getElementById('generateForm');
    form.addEventListener('change', () => {
        UserPreferences.saveFormData();
    });
});

// Export enhanced functions to global scope
window.NotificationManager = NotificationManager;
window.showWorldDetails = showWorldDetails;
window.closeModal = closeModal;
window.generateWorldWithProgress = generateWorldWithProgress;
window.UserPreferences = UserPreferences;
window.filterWorlds = filterWorlds;
