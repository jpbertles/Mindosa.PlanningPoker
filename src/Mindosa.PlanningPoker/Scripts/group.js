var group = (function () {

    function init(groupName) {

        var emptyCard = '&nbsp;';
        var planningPoker = new PokerViewModel();

        // Declare a proxy to reference the hub. 
        var planningHub = $.connection.planningHub;

        planningHub.client.reset = function(group) {
            planningPoker.cards(group.Cards);
            planningPoker.reset(group.GroupMembers);
        };

        planningHub.client.cardSelected = function(username, selectedCard) {
            planningPoker.selectCard(username, selectedCard);
        };

        planningHub.client.clear = function() {
            planningPoker.clear();
        };

        planningHub.client.updateCards = function(cards) {
            planningPoker.clear();
            planningPoker.cards(cards);
        };

        // Start the connection.
        $.connection.hub.start().done(function() {
            planningHub.server.joinGroup(groupName, planningPoker.myUserName()).done(function(userName) {
                planningPoker.myUserName(userName);
                $.cookie('username', userName);
            });
        });

        function UserModel(name, card, observerOnly) {
            var self = this;
            self.name = ko.observable(name);
            self.card = ko.observable(card);
            self.observerOnly = ko.observable(observerOnly);
            self.isSelected = ko.computed(function() {
                return self.card() != emptyCard;
            });
            self.getState = ko.computed(function() {
                if (self.observerOnly()) {
                    return "observer";
                } else if (self.card() != emptyCard) {
                    return "selected";
                }
                return "";
            })
        }

        function PokerViewModel() {
            var self = this;

            var priorUserName = $.cookie('username');

            self.users = ko.observableArray([]);
            self.cards = ko.observableArray([]);
            self.myUserName = ko.observable(priorUserName || '');
            self.showSettingsPanel = ko.observable(false);
            self.observerOnly = ko.observable(false);
            self.myCard = ko.observable(null);
            self.cardList = ko.computed({
                read: function() {
                    return self.cards();
                },
                write: function(value) {
                    self.cards(value.split(','));
                },
                owner: this
            });

            /* hub proxies */
            self.reset = function(groupMembers) {
                var mappedUsers = $.map(groupMembers, function(item) {
                    var username = $('<div />').text(item.Name).html();
                    if (username == self.myUserName()) {
                        self.myCard(item.SelectedCard);
                    }
                    return new UserModel(username, item.SelectedCard || '&nbsp;', item.IsObserverOnly);
                });
                self.users(mappedUsers);
            };

            self.selectCard = function(username, card) {
                for (var i = 0; i < self.users().length; i++) {
                    var user = self.users()[i];
                    if (user.name() == username) {
                        user.card(card);
                        return;
                    }
                }
            };
            self.clear = function() {
                for (var i = 0; i < self.users().length; i++) {
                    var user = self.users()[i];
                    user.card(emptyCard);
                }
                self.myCard(null);
            };

            /* display helpers */
            self.areAllCardsSelected = function() {
                for (var i = 0; i < self.users().length; i++) {
                    var user = self.users()[i];
                    if (!user.observerOnly() && user.card() == emptyCard) {
                        return false;
                    }
                }
                return true;
            };
            self.getCardValue = function(card) {
                return self.areAllCardsSelected() ? card : emptyCard;
            };
            self.isSelectedCard = function(card) {
                return self.myCard() == card;
            };


            /* event handlers */
            self.onUpdateSettingsClicked = function() {
                $.cookie('username', self.myUserName());
                $("#lnk-settings").click();
                planningHub.server.updateSettings(groupName, self.myUserName(), self.observerOnly(), self.cards());
            };
            self.onClearClicked = function() {
                planningHub.server.clear(groupName);
            };
            self.onCardClicked = function(card) {
                var originalCard = planningPoker.myCard();

                if (!planningPoker.areAllCardsSelected()) {
                    planningHub.server.selectCard(groupName, self.myUserName(), card).done(function(success) {
                        if (success) {
                            planningPoker.myCard(card);
                        } else {
                            planningPoker.myCard(originalCard);
                        }
                    });
                }
            };
            self.onSettingsClicked = function() {
                self.showSettingsPanel(!self.showSettingsPanel());
            };

        }


        ko.bindingHandlers.visible = {
            init: function(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var value = ko.utils.unwrapObservable(valueAccessor());

                var $element = $(element);

                if (value)
                    $element.show();
                else
                    $element.hide();
            },
            update: function(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var value = ko.utils.unwrapObservable(valueAccessor());

                var $element = $(element);


                var allBindings = allBindingsAccessor();

                // Grab data from binding property
                var duration = allBindings.duration || 500;
                var isCurrentlyVisible = !(element.style.display == "none");

                if (value && !isCurrentlyVisible)
                    $element.show(duration);
                else if ((!value) && isCurrentlyVisible)
                    $element.hide(duration);
            }
        };

        ko.applyBindings(planningPoker);
    }

    return {
      init: init  
    };
})();