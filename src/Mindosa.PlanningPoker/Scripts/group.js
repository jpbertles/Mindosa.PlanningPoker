var group = (function () {
    rivets.formatters.preventDefault = function (value) {
        return function (e) {
            e.preventDefault();
            value.call(this, e);
            return false;
        };
    };
    
    // Declare a proxy to reference the hub. 
    var planningHub = $.connection.planningHub;
    
    function areAllCardsSelected() {
        for (var i = 0; i < model.users.length; i++) {
            if (!model.users[i].isObserver && !model.users[i].card.selected) {
                return false;
            }
        }

        return true;
    }
    
    function setCards(cards) {
        model.cards = [];
        model.cardList = cards.join(',');
        
        for (var i = 0; i < cards.length; i++) {
            model.cards.push({
                name: cards[i],
                isSelected: false
            });
        }
    }
    
    var init = function (groupName) {
        model.group = groupName;
        var priorUserName = $.cookie('username');
        model.myUserName = priorUserName || '';
        
        planningHub.client.reset = function (group) {
            setCards(group.Cards);
            
            model.users = [];
            for (var i = 0; i < group.GroupMembers.length; i++) {
                model.users.push({
                    name: group.GroupMembers[i].Name,
                    isObserver: group.GroupMembers[i].IsObserverOnly,
                    card: {
                        name: group.GroupMembers[i].SelectedCard,
                        selected: group.GroupMembers[i].SelectedCard && group.GroupMembers[i].SelectedCard != ''
                    }
                });
            }
            
            model.showSelections = areAllCardsSelected();
        };

        planningHub.client.cardSelected = function(username, selectedCard) {
            for (var i = 0; i < model.users.length; i++) {
                if (model.users[i].name == username) {
                    model.users[i].card.name = selectedCard;
                    model.users[i].card.selected = selectedCard && selectedCard != '';
                }
            }

            model.showSelections = areAllCardsSelected();
        };

        planningHub.client.clear = function() {
            for (var i = 0; i < model.users.length; i++) {
                model.users[i].card.name = '';
                model.users[i].card.selected = false;
            }
            model.myCard = null;
            
            for (var j = 0; j < model.cards.length; j++) {
                model.cards[j].selected = false;
            }

            model.showSelections = false;
        };

        planningHub.client.updateCards = function(cards) {
            planningHub.client.clear();
            setCards(cards);
        };

        // Start the connection.
        $.connection.hub.start().done(function() {
            planningHub.server.joinGroup(model.group, model.myUserName).done(function(userName) {
                $.cookie('username', userName);
                model.myUserName = userName;
            });
        });
    };

    var model = {
        group: '',
        users: [],
        cards: [],
        myUserName: '',
        cardList:'',
        observerOnly: false,
        myCard: null,
        showSelections: false
    };

    var viewModel = {
        onCardClicked: function (el, args) {
            if (!areAllCardsSelected()) {
                planningHub.server.selectCard(model.group, model.myUserName, args.card.name).done(function (success) {
                    if (success) {
                        model.myCard = args.card.name;

                        for (var i = 0; i < model.cards.length; i++) {
                            model.cards[i].selected = model.cards[i].name == model.myCard;
                        }
                    } else {
                        //do nothing
                    }
                });
            }
        },
        onClearClicked: function(el, args) {
            planningHub.server.clear(model.group);
        },
        onSettingsClicked: function(el, args) {
            $("#settings-panel").slideToggle(300);
        },
        onUpdateSettingsClicked: function (el, args) {
            $.cookie('username', model.myUserName);
            planningHub.server.updateSettings(model.group, model.myUserName, model.observerOnly, model.cardList.split(','));
            $("#settings-panel").slideToggle(300);
        }
    };
    
    return {
        model: model,
        viewModel: viewModel,
        init: init
    };
})();