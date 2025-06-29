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

// Enhanced Student Dashboard Interactivity
document.addEventListener('DOMContentLoaded', function() {
    
    // Enhanced Stats Cards Animation
    function animateStatsCards() {
        const statsCards = document.querySelectorAll('.stats-card');
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry, index) => {
                if (entry.isIntersecting) {
                    setTimeout(() => {
                        entry.target.style.opacity = '1';
                        entry.target.style.transform = 'translateY(0)';
                        
                        // Animate the number counting
                        const numberElement = entry.target.querySelector('.stats-info h3');
                        if (numberElement) {
                            animateNumber(numberElement);
                        }
                    }, index * 150);
                }
            });
        }, { threshold: 0.1 });
        
        statsCards.forEach(card => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(30px)';
            card.style.transition = 'all 0.6s cubic-bezier(0.4, 0, 0.2, 1)';
            observer.observe(card);
        });
    }
    
    // Number counting animation
    function animateNumber(element) {
        const finalNumber = parseInt(element.textContent);
        const duration = 1500;
        const startTime = performance.now();
        
        function updateNumber(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function for smooth animation
            const easeOut = 1 - Math.pow(1 - progress, 3);
            const currentNumber = Math.floor(finalNumber * easeOut);
            
            element.textContent = currentNumber;
            
            if (progress < 1) {
                requestAnimationFrame(updateNumber);
            } else {
                element.textContent = finalNumber;
            }
        }
        
        if (finalNumber > 0) {
            element.textContent = '0';
            requestAnimationFrame(updateNumber);
        }
    }
    
    // Progress ring animation
    function animateProgressRings() {
        const progressRings = document.querySelectorAll('.progress-circle circle:last-child');
        
        progressRings.forEach(ring => {
            const dashArray = ring.getAttribute('stroke-dasharray');
            if (dashArray) {
                const [progress, total] = dashArray.split(' ').map(Number);
                
                // Start from 0 and animate to the target value
                ring.style.strokeDasharray = `0 ${total}`;
                ring.style.transition = 'stroke-dasharray 2s cubic-bezier(0.4, 0, 0.2, 1)';
                
                setTimeout(() => {
                    ring.style.strokeDasharray = `${progress} ${total}`;
                }, 500);
            }
        });
    }
    
    // Timeline progress animation
    function animateTimeline() {
        const timelineProgress = document.querySelector('.timeline-progress');
        if (timelineProgress) {
            const targetWidth = timelineProgress.style.width;
            timelineProgress.style.width = '0%';
            timelineProgress.style.transition = 'width 2s cubic-bezier(0.4, 0, 0.2, 1)';
            
            setTimeout(() => {
                timelineProgress.style.width = targetWidth;
            }, 800);
        }
    }
    
    // Enhanced action card hover effects
    function enhanceActionCards() {
        const actionCards = document.querySelectorAll('.action-card:not(.disabled)');
        
        actionCards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-6px) scale(1.02)';
                
                // Add ripple effect
                const ripple = document.createElement('div');
                ripple.style.cssText = `
                    position: absolute;
                    top: 50%;
                    left: 50%;
                    width: 0;
                    height: 0;
                    background: radial-gradient(circle, rgba(111, 179, 211, 0.1) 0%, transparent 70%);
                    border-radius: 50%;
                    transform: translate(-50%, -50%);
                    pointer-events: none;
                    animation: ripple 0.6s ease-out;
                `;
                
                this.style.position = 'relative';
                this.appendChild(ripple);
                
                setTimeout(() => {
                    if (ripple.parentNode) {
                        ripple.parentNode.removeChild(ripple);
                    }
                }, 600);
            });
            
            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0) scale(1)';
            });
        });
    }
    
    // Add loading state simulation for demonstration
    function simulateLoading() {
        const contentCards = document.querySelectorAll('.content-card, .management-card');
        
        contentCards.forEach((card, index) => {
            card.classList.add('loading');
            
            setTimeout(() => {
                card.classList.remove('loading');
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';
                card.style.transition = 'all 0.5s cubic-bezier(0.4, 0, 0.2, 1)';
                
                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, 100 + (index * 100));
            }, 300 + (index * 100));
        });
    }
    
    // Enhanced tooltip functionality
    function addTooltips() {
        const elements = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        elements.forEach(element => {
            new bootstrap.Tooltip(element, {
                animation: true,
                delay: { show: 300, hide: 100 },
                html: true
            });
        });
    }
    
    // Smooth scroll for anchor links
    function setupSmoothScroll() {
        const links = document.querySelectorAll('a[href^="#"]');
        links.forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }
    
    // Auto-refresh timestamp
    function updateTimestamp() {
        const timestampElement = document.getElementById('current-datetime');
        if (timestampElement) {
            const now = new Date();
            timestampElement.textContent = now.toLocaleDateString('en-US', {
                month: 'short',
                day: 'numeric',
                hour: 'numeric',
                minute: '2-digit'
            });
        }
    }
    
    // Enhanced card interactions
    function enhanceCardInteractions() {
        const cards = document.querySelectorAll('.content-card, .stats-card, .management-card');
        
        cards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-4px)';
            });
            
            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
            });
        });
    }
    
    // Initialize all enhancements
    function init() {
        // Simulate loading for visual effect
        simulateLoading();
        
        // Set up animations with delays for better UX
        setTimeout(() => {
            animateStatsCards();
            animateProgressRings();
            animateTimeline();
            enhanceActionCards();
            enhanceCardInteractions();
            addTooltips();
            setupSmoothScroll();
            updateTimestamp();
            
            // Update timestamp every minute
            setInterval(updateTimestamp, 60000);
        }, 500);
    }
    
    // Initialize everything
    init();
    
    // Add CSS animations for ripple effect
    if (!document.getElementById('dynamic-animations')) {
        const style = document.createElement('style');
        style.id = 'dynamic-animations';
        style.textContent = `
            @@keyframes ripple {
                0% {
                    width: 0;
                    height: 0;
                    opacity: 1;
                }
                100% {
                    width: 200px;
                    height: 200px;
                    opacity: 0;
                }
            }
            
            .action-card {
                overflow: hidden;
            }
        `;
        document.head.appendChild(style);
    }
});

// Performance optimization: Use passive event listeners where appropriate
if ('addEventListener' in window) {
    const options = { passive: true };
    
    // Add performance-optimized scroll listener
    window.addEventListener('scroll', function() {
        // Throttle scroll events for better performance
        if (!window.scrolling) {
            window.scrolling = true;
            requestAnimationFrame(() => {
                // Add any scroll-based animations here
                window.scrolling = false;
            });
        }
    }, options);
}
