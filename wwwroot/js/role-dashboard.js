/* 
 * Role Dashboard Shared JavaScript
 * Common functionality for all role dashboards
 */

// Update current date and time
function updateDateTime() {
    const now = new Date();
    const options = { 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    };
    const datetimeElement = document.getElementById('current-datetime');
    if (datetimeElement) {
        datetimeElement.textContent = now.toLocaleDateString('en-US', options);
    }
}

// Initialize dashboard functionality
function initializeDashboard() {
    // Update datetime
    updateDateTime();
    setInterval(updateDateTime, 60000); // Update every minute
    
    // Add smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });
    
    // Add hover effects to cards
    document.querySelectorAll('.stats-card, .management-card, .content-card, .nav-card').forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
    
    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined') {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// Wait for DOM to be ready
document.addEventListener('DOMContentLoaded', function() {
    initializeDashboard();
});

// Handle window resize for responsive features
window.addEventListener('resize', function() {
    // Add any responsive adjustments here
});

// Utility function to show notifications
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

// Utility function to animate counters
function animateCounter(element, start, end, duration) {
    let startTimestamp = null;
    const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        const current = Math.floor(progress * (end - start) + start);
        element.textContent = current.toLocaleString();
        if (progress < 1) {
            window.requestAnimationFrame(step);
        }
    };
    window.requestAnimationFrame(step);
}

// Initialize counter animations for stats cards
function initializeCounters() {
    document.querySelectorAll('.stats-info h3').forEach(counter => {
        const finalNumber = parseInt(counter.textContent.replace(/,/g, ''));
        if (!isNaN(finalNumber)) {
            animateCounter(counter, 0, finalNumber, 2000);
        }
    });
}

// Call counter initialization after a small delay to ensure proper rendering
setTimeout(initializeCounters, 500);
